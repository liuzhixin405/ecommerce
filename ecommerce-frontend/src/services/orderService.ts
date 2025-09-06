import apiClient from '../api/client';
import { Order, CreateOrderDto } from '../interfaces';

export const getOrders = async (): Promise<Order[]> => {
  try {
    const response = await apiClient.get<Order[]>('/orders');
    return response.data;
  } catch (error) {
    console.error('Failed to fetch orders:', error);
    throw error;
  }
};

export const getOrderById = async (id: string): Promise<Order> => {
  try {
    const response = await apiClient.get<Order>(`/orders/${id}`);
    return response.data;
  } catch (error) {
    console.error('Failed to fetch order:', error);
    throw error;
  }
};

export const createOrder = async (orderData: CreateOrderDto): Promise<Order> => {
  try {
    const response = await apiClient.post<Order>('/orders', orderData);
    return response.data;
  } catch (error) {
    console.error('Failed to create order:', error);
    throw error;
  }
};

export const updateOrderStatus = async (id: string, status: string): Promise<Order> => {
  try {
    const response = await apiClient.put<Order>(`/orders/${id}/status`, { status });
    return response.data;
  } catch (error) {
    console.error('Failed to update order status:', error);
    throw error;
  }
};

export const cancelOrder = async (id: string): Promise<Order> => {
  try {
    const response = await apiClient.put<Order>(`/orders/${id}/cancel`);
    return response.data;
  } catch (error) {
    console.error('Failed to cancel order:', error);
    throw error;
  }
};