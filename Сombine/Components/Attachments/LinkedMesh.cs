namespace Сombine.Components.Attachments
{
    public class LinkedMesh : Attachment
    {
        public string Path { get; set; }

        public string Skin { get; set; }

        public string Parent { get; set; }

        public bool Deform { get; set; }

        public object Color { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }
    }
}