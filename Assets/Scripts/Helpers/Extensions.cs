using UnityEngine;

namespace Helpers
{
    public static class Extensions
    {
        
    }
    
    public static class LayerMaskExtensions
    {
        public static bool Includes(this LayerMask mask, int layer)
        {
            return (mask.value & 1 << layer) > 0;
        }
    }
}