using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Сombine.Components;
using Сombine.Components.Attachments;
using Сombine.Resolvers;
using Сombine.Units;

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
		
		
		public bool CompareSlots(Slot[] other, out Slot[] identical)
		{
			identical = Array.Empty<Slot>();
			var equalSlots = new List<Slot>();
			
			for (int i = 0; i < Slots.Length; i++)
			{
				var currentSlot = Slots[i];
				for (int j = 0; j < other.Length; j++)
				{
					if (currentSlot.Equals(other[j]))
					{
						if(CompareAttachments(currentSlot.Attachment, other[j].Attachment))
						{
							equalSlots.Add(currentSlot);
							break;
						}
					}
				}
			}
			
			identical = equalSlots.ToArray();
			return identical.Length > 0;
		}
		
		public void AssignSlotDataToOther(SpineDocument document, Slot[] identical)
		{
			var other = document.Slots;
			document.Bones = Bones;
			var imageScale = CalculateImageSizeRatio(document.ActualWidth, document.ActualHeight);
			
			
			//BoneResolver.AdjustBonePositions(document.Bones, scale.X, scale.Y);
			
			for (int i = 0; i < identical.Length; i++)
			{
				for (int j = 0; j < other.Length; j++)
				{
					if (identical[i].Equals(other[j]))
					{
						var thisAttachment = identical[i].Attachment;
						var otherAttachment = other[j].Attachment;
						
						if (thisAttachment is IVertexIncludingAttachment thisMesh
						    && otherAttachment is IVertexIncludingAttachment otherMesh)
						{
							var scale = CalculateAttachmentSizeRatio(thisMesh, otherMesh);
							BoneResolver.AdjustBonePositions(thisMesh.VertexCollection.GetRelativeBones(), scale.X, scale.Y, true);
							
							AssignVerticesToOther(
								thisMesh, 
								otherMesh, 
								scale);
						}
						else
						{
							AssignPositionToOther(
								(IVertexExcludingAttachment) thisAttachment, 
								(IVertexExcludingAttachment) otherAttachment, 
								imageScale);
						}
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

		public Vertex CalculateAttachmentSizeRatio(IVertexIncludingAttachment original, IVertexIncludingAttachment target)
		{
			var originalSize = ComputeActualBonedAttachmentSize(original);
			var targetSize = ComputeActualAttachmentSize(target);
			
			return Vertex.CalculateRatio(originalSize.X, originalSize.Y, targetSize.X, targetSize.Y);
		}
		
		private bool CheckForTiedBones()
		{
			foreach (Slot slot in Slots)
			{
				if (slot.Attachment is IVertexIncludingAttachment attachment)
					if (attachment.VertexCollection[0].Vertices[0].Connected)
					{
						return true;
					}
			}
			return false;
		}
		
		private bool CompareAttachments(Attachment inspected, Attachment other)
		{
			//Todo: Добавить проверки на другие компоненты имеющие вершины с весами
			if (inspected is IVertexIncludingAttachment mesh1 && other is IVertexIncludingAttachment mesh2)
				return mesh1.VertexCollection.Count == mesh2.VertexCollection.Count;
			
			return inspected.GetType() == other.GetType();
		}
		
		private void AssignVerticesToOther(IVertexIncludingAttachment meshFrom, IVertexIncludingAttachment meshTo, Vertex scaling)
		{
			for (int i = 0; i < meshFrom.VertexCollection.Count; i++)
			{
				var borrowedVertices = meshFrom.VertexCollection[i];
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

		private void AssignPositionToOther(IVertexExcludingAttachment meshFrom, IVertexExcludingAttachment meshTo, Vertex scaling)
		{
			meshFrom.Adjust(scaling.X, scaling.Y);
			meshTo = meshFrom;
		}
		
		private Vertex ComputeActualImageSize()
		{
			List<float> vertexWidth  = new List<float>(); 
			List<float> vertexHeight = new List<float>();
			
			foreach (var slot in Slots)
			{
				if (slot.Attachment is IVertexIncludingAttachment mesh)
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
		
		private Vertex ComputeActualBonedAttachmentSize(IVertexIncludingAttachment attachment)
		{
			List<float> boneWidth  = new List<float>(); 
			List<float> boneHeight = new List<float>();

			foreach (VertexNode node in attachment.VertexCollection)
			{
				foreach (Vertex vertex in node)
				{
					if (vertex.Parent.Length > 0.1f)
					{
						float newX = vertex.Parent.X + (float)Math.Cos(vertex.Parent.Rotation) * vertex.Parent.Length;
						float newY = vertex.Parent.Y + (float)Math.Sin(vertex.Parent.Rotation) * vertex.Parent.Length;
						boneWidth.Add(newX);
						boneHeight.Add(newY);
					}
					
					boneWidth.Add(vertex.Parent.X);
					boneHeight.Add(vertex.Parent.Y);
				}
			}
			
			var meshSize = ComputeActualAttachmentSize(attachment);

			boneWidth.Add(meshSize.X);
			boneHeight.Add(meshSize.Y);
				
			var x = Math.Abs(boneWidth.Max()) + Math.Abs(boneWidth.Min());
			var y = Math.Abs(boneHeight.Max()) + Math.Abs(boneHeight.Min());
			
			return new Vertex(x, y);
		}
		
		private Vertex ComputeActualAttachmentSize(IVertexIncludingAttachment attachment)
		{
			List<float> vertexWidth  = new List<float>(); 
			List<float> vertexHeight = new List<float>();
			
			foreach (VertexNode node in attachment.VertexCollection)
			{
				foreach (Vertex vertex in node)
				{
					vertexWidth.Add(vertex.X);
					vertexHeight.Add(vertex.Y);
				}
			}

			var x = Math.Abs(vertexWidth.Max()) + Math.Abs(vertexWidth.Min());
			var y = Math.Abs(vertexHeight.Max()) + Math.Abs(vertexHeight.Min());
			
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