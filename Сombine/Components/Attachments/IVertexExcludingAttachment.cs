namespace Сombine.Components.Attachments
{
    public interface IVertexExcludingAttachment
    {
        float X { get; set; }
        float Y { get; set; }

        void Adjust(float x, float y);
    }
}