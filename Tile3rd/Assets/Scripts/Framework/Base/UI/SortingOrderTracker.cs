using UnityEngine;

namespace CSFramework
{
    public class SortingOrderTracker : CSBehaviour
    {
        public int LastBindingSortingOrder = 0;
        public Renderer Renderer = null;
        public Canvas Canvas = null;

        public void BindSortingOrder (int sorting_order)
        {
            var order_offset = sorting_order - LastBindingSortingOrder;
            if (order_offset != 0)
            {
                if (Renderer)
                {
                    Renderer.sortingOrder += order_offset;
                }
                else if (Canvas)
                {
                    // Canvas.sortingOrder += order_offset;
                    Canvas.sortingOrder = sorting_order;
                }
                else
                {
                    if (TryGetComponent<Renderer>(out Renderer))
                    {
                        Renderer.sortingOrder += order_offset;
                    }
                    else if (TryGetComponent<Canvas>(out Canvas))
                    {
                        // Canvas.sortingOrder += order_offset;
                        Canvas.sortingOrder = sorting_order;
                    }
                }
                LastBindingSortingOrder = sorting_order;
            }
        }
    }
}