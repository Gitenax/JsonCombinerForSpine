using System;
using Сombine.Components;

namespace Сombine.Resolvers
{
    internal static class BoneResolver
    {
        public static void AssignParentToBones(Bone[] bones)
        {
            foreach (var bone in bones)
            {
                if(string.IsNullOrEmpty(bone.ParentName)) continue;
                
                var parent = FindByName(bones, bone.ParentName);
                bone.SetParent(parent);
            }

            AssignBoneIndex(bones);
        }
        

        /// <exception cref="NullReferenceException" />
        public static Bone FindByName(Bone[] bones, string name)
        {
            foreach (var bone in bones)
                if (bone.Name.Equals(name)) return bone;
            
            return default;
        }

        private static void AssignBoneIndex(Bone[] bones)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].Index = i;
            }
        }
    }
}