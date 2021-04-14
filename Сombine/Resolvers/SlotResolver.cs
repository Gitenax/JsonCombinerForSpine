using Сombine.Components;

namespace Сombine.Resolvers
{
    internal class SlotResolver
    {
        public static void AssignBonesToSlots(Bone[] bones, Slot[] slots)
        {
            foreach (var slot in slots)
            {
                slot.Bone = BoneResolver.FindByName(bones, slot.BoneName);
            }
        }
    }
}