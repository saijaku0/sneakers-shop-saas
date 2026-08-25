import { api } from "@/shared/api";
import {
  type AddToCartRequest,
  type CartItem,
  type UpdateCartQuantityRequest,
} from "../lib/types";
import { setCartFromServer } from "../model/cart-slice";

interface CartResponse {
  items: CartItem[];
  totalPrice: number;
}

export const cartApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getCart: builder.query<CartItem[], void>({
      query: () => ({
        url: "/cart",
        method: "GET",
      }),
      transformResponse: (response: CartResponse) => response.items,
      async onQueryStarted(_, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setCartFromServer(data));
        } catch (error) {
          console.error("Failed to fetch cart:", error);
        }
      },
      providesTags: ["Cart"],
    }),
    addToCart: builder.mutation<void, AddToCartRequest>({
      query: (body) => ({
        url: "/cart/items",
        method: "POST",
        body,
      }),
      invalidatesTags: ["Cart"],
    }),
    removeFromCart: builder.mutation<void, string>({
      query: (warehouseItemId) => ({
        url: `/cart/items/${warehouseItemId}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Cart"],
    }),
    updateCartItemQuantity: builder.mutation<void, UpdateCartQuantityRequest>({
      query: ({ warehouseItemId, quantity }) => ({
        url: `/cart/items/${warehouseItemId}`,
        method: "PUT",
        body: { quantity },
      }),
      invalidatesTags: ["Cart"],
    }),
    syncCart: builder.mutation<null, AddToCartRequest[]>({
      queryFn: async (items, _api, _extraOptions, baseQuery) => {
        for (const item of items) {
          const result = await baseQuery({
            url: "/cart/items",
            method: "POST",
            body: item,
          });

          if (result.error) {
            return { error: result.error };
          }
        }

        return { data: null };
      },
      invalidatesTags: ["Cart"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetCartQuery,
  useLazyGetCartQuery,
  useAddToCartMutation,
  useRemoveFromCartMutation,
  useUpdateCartItemQuantityMutation,
  useSyncCartMutation,
} = cartApi;
