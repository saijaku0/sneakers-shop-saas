import { api } from "@/shared/api";
import type {
  CreateOrderRequest,
  OrderDetail,
  OrderSummary,
} from "../lib/types";

export const orderApi = api.injectEndpoints({
  endpoints: (builder) => ({
    createOrder: builder.mutation<string, CreateOrderRequest>({
      query: (body) => ({
        url: "/orders",
        method: "POST",
        body,
      }),
      invalidatesTags: ["Cart"],
    }),
    getOrders: builder.query<OrderSummary[], void>({
      query: () => ({
        url: "/orders",
        method: "GET",
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.map((order) => ({
                type: "Orders" as const,
                id: order.id,
              })),
              { type: "Orders" as const, id: "LIST" },
            ]
          : [{ type: "Orders" as const, id: "LIST" }],
    }),
    getOrderById: builder.query<OrderDetail, string>({
      query: (id) => ({
        url: `/orders/${id}`,
        method: "GET",
      }),
      providesTags: (_result, _error, id) => [{ type: "Orders", id }],
    }),
  }),
  overrideExisting: false,
});

export const {
  useCreateOrderMutation,
  useGetOrdersQuery,
  useGetOrderByIdQuery,
} = orderApi;
