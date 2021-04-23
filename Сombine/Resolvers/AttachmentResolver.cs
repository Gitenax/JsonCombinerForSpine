using Сombine.Components;
using Сombine.Components.Attachments;
using Сombine.Units;

namespace Сombine.Resolvers
{
    internal static class AttachmentResolver
    {
        public static void VerifyAttachmentProperties(Attachment attachment)
        {
            if (attachment is IVertexIncludingAttachment mesh)
            {
                VertexCollection collection = new VertexCollection(mesh.VertexCount);
                VertexNode[] nodes;

                if (mesh.Vertices.Length > mesh.VertexCount * 2)
                    nodes = CreateWeightedNodes(mesh.Vertices, mesh.VertexCount);
                else
                    nodes = CreateSimpleNodes(mesh.Vertices, mesh.VertexCount);
                
                collection.AddRange(nodes);
                
                mesh.VertexCollection = collection;
            }
        }
        
        private static VertexNode[] CreateWeightedNodes(float[] vertices, int hull)
        {
            var nodes = new VertexNode[hull];
            int currentIndex = 0;

            for (int i = 0; i < nodes.Length; i++)
            {
                int vertexCount = (int) vertices[currentIndex]; // Количество костей
                
                Vertex[] vtx = new Vertex[vertexCount];
                for (int j = 0; j < vtx.Length; j++)
                {
                    int bone     = (int)vertices[currentIndex + 1];
                    float x      = vertices[currentIndex + 2];
                    float y      = vertices[currentIndex + 3];
                    float weight = vertices[currentIndex + 4];
                    
                    vtx[j] = new Vertex(bone, x, y, weight);
                    currentIndex += 4;
                }
                nodes[i] = new VertexNode(vtx);
                currentIndex++;
            }
            
            return nodes;
        }

        private static VertexNode[] CreateSimpleNodes(float[] vertices, int hull)
        {
            var nodes = new VertexNode[hull];
            int currentIndex = 0;

            for (int i = 0; i < nodes.Length; i++)
            {
                float x = vertices[currentIndex];
                float y = vertices[currentIndex + 1];
                
                nodes[i] = new VertexNode(new Vertex(x, y));
                currentIndex += 2;
            }
            
            return nodes;
        }
        }
}