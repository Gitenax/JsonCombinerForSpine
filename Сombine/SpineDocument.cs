using Сombine.Components;
using Сombine.Utils;
using Сombine.Components.Attachments;

namespace Сombine
{
    public class SpineDocument
    {
        public Skeleton Skeleton { get; set; }
        public Bone[] Bones { get; set; }
        public Slot[] Slots { get; set; }
        public Skin[] Skins { get; set; }
        public object Animations { get; set; }


        public bool CheckForTiedBones()
        {
            foreach (Slot slot in Slots)
            {
                if (slot.Attachment is Mesh mesh)
                    return mesh.VertexCollection[0].Vertices[0].Connected;
            }

            return false;
        }

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

        public void AssignSlotDataToOther(Slot[] other)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                for (int j = 0; j < other.Length; j++)
                {
                    if (Slots[i].Equals(other[j]))
                    {
                        var currentMesh = (Mesh)Slots[i].Attachment;
                        var inspectedMesh = (Mesh)other[j].Attachment;
                        AssignVerticesToOther(currentMesh, inspectedMesh);
                    }
                }
            }
        }

        
        private bool CompareAttachments(Attachment inspected, Attachment other)
        {
            if (inspected is Mesh mesh1 && other is Mesh mesh2)
            {
                return mesh1.VertexCollection.Count == mesh2.VertexCollection.Count;
            }
            
            if(inspected.GetType() == other.GetType())
            {
                return true;
            }
            
            return false;
        }

        private void AssignVerticesToOther(Mesh from, Mesh to)
        {
            for (int i = 0; i < from.VertexCollection.Count; i++)
            {
                var borrowedVertices = from.VertexCollection[i];
                var oldVertices = to.VertexCollection[i];
                
                /*var newVertices = new List<Vertex>();
                foreach (Vertex vertex in borrowedVertices)
                {
                    Vertex newVertex = new Vertex(
                        vertex.BoneIndex, 
                        vertex.X,
                        vertex.Y,
                        vertex.Weight);
                    
                    newVertices.Add(newVertex);
                }*/
                
                to.VertexCollection[i] = new VertexNode(borrowedVertices.Vertices);
            }
        }
    }


}