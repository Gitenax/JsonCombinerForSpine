using Сombine.Units;

namespace Сombine.Components.Attachments
{
    /// <summary>
    /// Объект содержащий коллекцию вершин
    /// </summary>
    public interface IVertexIncludingAttachment
    {
        float[] Vertices { get; set; }
        VertexCollection VertexCollection { get; set; }
        int VertexCount { get; }
    }
}