using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Сombine.Components;
using Сombine.Utils;
using Сombine.Components.Attachments;
using Сombine.Resolvers;

namespace Сombine
{
	public class SpineDocument
	{
		public SpineDocument(){}

		public SpineDocument(Skeleton skeleton, Bone[] bones, Slot[] slots, Skin[] skins, object animations)
		{
			Skeleton = skeleton;
			Bones = bones;
			Slots = slots;
			Skins = skins;
			Animations = animations;
			
			HasTiedBones = CheckForTiedBones();
			
			var imageSize = ComputeActualImageSize();
			var bonedSize = HasTiedBones ? ComputeActualBonedImageSize() : Vertex.Zero;
			
			ActualHeight = imageSize.Y;
			ActualWidth = imageSize.X;
			ActualBonedHeight = bonedSize.Y;
			ActualBonedWidth = bonedSize.X;
		}
		
		
		public IEnumerable GetTableData()
		{
			List<Tuple<string, string, string>> tree = new List<Tuple<string, string, string>>();
			
			foreach (var slot in Slots)
			{
				if(slot.Attachment == null) continue;
				tree.Add(new Tuple<string, string, string>(slot.Name, slot.Attachment.Type, slot.Attachment.Name));
			}

			foreach (var o in tree)
			{
				yield return o;
			}
		}
		
		
		public Skeleton Skeleton { get; set; }
		public Bone[] Bones { get; set; }
		public Slot[] Slots { get; set; }
		public Skin[] Skins { get; set; }
		public object Animations { get; set; }

		[JsonIgnore]
		public float ActualWidth { get; }
		
		[JsonIgnore]
		public float ActualHeight { get; }
		
		[JsonIgnore]
		public float ActualBonedWidth { get; }
		
		[JsonIgnore]
		public float ActualBonedHeight { get; }
		
		[JsonIgnore]
		public bool HasTiedBones { get; }
		
		
		public bool CompareSlots(Slot[] other)
		{
			if(Slots.Length != other.Length) return false;
			
			for (int i = 0; i < Slots.Length; i++)
			{
				var currentSlot = Slots[i];
				bool hasEquals = false;
				for (int j = 0; j < other.Length; j++)
				{
					if (currentSlot.Equals(other[j]))
					{
						if(CompareAttachments(currentSlot.Attachment, other[j].Attachment))
						{
							hasEquals = true;
							break;
						}
						return false;
					}
				}
				if (!hasEquals) return false;
			}
			
			return true;
		}
		
		public void AssignSlotDataToOther(SpineDocument document)
		{
			var other = document.Slots;
			var scale = CalculateImageSizeRatio(document.ActualWidth, document.ActualHeight);
			
			document.Bones = Bones;
			BoneResolver.AdjustBonePositions(document.Bones, scale.X, scale.Y);
			
			for (int i = 0; i < Slots.Length; i++)
			{
				for (int j = 0; j < other.Length; j++)
				{
					if (Slots[i].Equals(other[j]))
					{
						var currentMesh = (Mesh)Slots[i].Attachment;
						var inspectedMesh = (Mesh)other[j].Attachment;
						AssignVerticesToOther(currentMesh, inspectedMesh, scale);
					}
				}
			}
		}

		public Vertex CalculateImageSizeRatio(float width, float height)
		{
			if (HasTiedBones)
				return Vertex.CalculateRatio(ActualBonedWidth, ActualBonedHeight, width, height);
			
			return Vertex.CalculateRatio(ActualWidth, ActualHeight, width, height);
		}

		private bool CheckForTiedBones()
		{
			foreach (Slot slot in Slots)
			{
				if (slot.Attachment is Mesh mesh)
					return mesh.VertexCollection[0].Vertices[0].Connected;
			}
			return false;
		}
		
		private bool CompareAttachments(Attachment inspected, Attachment other)
		{
			//Todo: Добавить проверки на другие компоненты имеющие вершины с весами
			if (inspected is Mesh mesh1 && other is Mesh mesh2)
				return mesh1.VertexCollection.Count == mesh2.VertexCollection.Count;
			
			return inspected.GetType() == other.GetType();
		}
		
		private void AssignVerticesToOther(Mesh meshFrom, Mesh meshTo, Vertex scaling)
		{
			for (int i = 0; i < meshFrom.VertexCollection.Count; i++)
			{
				var borrowedVertices = meshFrom.VertexCollection[i];
				var oldVertices = meshTo.VertexCollection[i];
				var oldPos = (oldVertices[0].X, oldVertices[0].Y);
				
				var newVertices = new List<Vertex>();
				foreach (Vertex vertex in borrowedVertices)
				{
					vertex.Adjust(scaling.X, scaling.Y);
					var newVertex = new Vertex(vertex.BoneIndex, vertex.X, vertex.Y, vertex.Weight);
					
					newVertices.Add(newVertex);
				}
				
				meshTo.VertexCollection[i] = new VertexNode(newVertices.ToArray());
			}
		}
		
		private Vertex ComputeActualImageSize()
		{
			List<float> vertexWidth  = new List<float>(); 
			List<float> vertexHeight = new List<float>();
			
			foreach (var slot in Slots)
			{
				if (slot.Attachment is Mesh mesh)
				{
					foreach (VertexNode node in mesh.VertexCollection)
					{
						vertexHeight.Add(GetPointPosition(node.Vertices).Y);
						vertexWidth.Add(GetPointPosition(node.Vertices).X);
					}
				}
			}
			var x = Math.Abs(vertexWidth.Max()) + Math.Abs(vertexWidth.Min());
			var y = Math.Abs(vertexHeight.Max()) + Math.Abs(vertexHeight.Min());
			
			return new Vertex(x, y);
		}

		private Vertex ComputeActualBonedImageSize()
		{
			List<float> boneWidth  = new List<float>(); 
			List<float> boneHeight = new List<float>();

			foreach (var bone in Bones)
			{
				boneWidth.Add(bone.X);
				boneHeight.Add(bone.Y);
			}
			var x = Math.Abs(boneWidth.Max()) + Math.Abs(boneWidth.Min());
			var y = Math.Abs(boneHeight.Max()) + Math.Abs(boneHeight.Min());
			
			return new Vertex(x, y);
		}
		
		private Vertex GetPointPosition(Vertex[] vertices)
		{
			if(HasTiedBones)
			{
				var (x, y) = (0f, 0f);
				foreach (var vertex in vertices)
				{
					x += (Bones[vertex.BoneIndex].X + vertex.X) * vertex.Weight;
					y += (Bones[vertex.BoneIndex].Y + vertex.Y) * vertex.Weight;
				}
				return new Vertex(x, y);
			}
			return new Vertex(vertices[0].X, vertices[0].Y);
		}
	}
}