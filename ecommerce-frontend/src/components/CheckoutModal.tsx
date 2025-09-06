import React, { useState } from 'react';
import { X } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { CreateOrderDto, CreateOrderItemDto } from '../interfaces';
import { cartService } from '../services/cartService';
import { createOrder } from '../services/orderService';
import { formatPrice } from '../utils/format';

interface CheckoutModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const CheckoutModal: React.FC<CheckoutModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const { user } = useAuth();
  const [formData, setFormData] = useState({
    customerName: user?.firstName && user?.lastName ? `${user.firstName} ${user.lastName}` : '',
    phoneNumber: user?.phoneNumber || '',
    shippingAddress: user?.address || '',
    paymentMethod: 'CreditCard',
    notes: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const cartItems = cartService.getCart();
  const total = cartService.getCartTotal();

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!formData.customerName || !formData.phoneNumber || !formData.shippingAddress) {
      setError('请填写所有必填字段');
      return;
    }

    if (cartItems.length === 0) {
      setError('购物车为空');
      return;
    }

    setLoading(true);
    try {
      const orderData: CreateOrderDto = {
        userId: user?.id,
        customerName: formData.customerName,
        phoneNumber: formData.phoneNumber,
        shippingAddress: formData.shippingAddress,
        paymentMethod: formData.paymentMethod,
        notes: formData.notes,
        items: cartItems.map(item => ({
          productId: item.product.id,
          quantity: item.quantity
        } as CreateOrderItemDto))
      };

      const order = await createOrder(orderData);
      
      // Process payment (simulated - this would be handled by payment service)
      console.log('Processing payment for order:', order.id, 'with method:', formData.paymentMethod);

      // Clear cart after successful order
      cartService.clearCart();
      
      onSuccess();
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : '结算失败');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">结算</h2>
          <button onClick={onClose} className="text-gray-500 hover:text-gray-700">
            <X size={24} />
          </button>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="border rounded-lg p-4">
            <h3 className="font-semibold mb-3">订单摘要</h3>
            <div className="space-y-2">
              {cartItems.map((item) => (
                <div key={item.product.id} className="flex justify-between">
                  <span>{item.product.name} x {item.quantity}</span>
                  <span>{formatPrice(item.product.price * item.quantity)}</span>
                </div>
              ))}
              <div className="border-t pt-2 mt-2">
                <div className="flex justify-between font-bold">
                  <span>总计:</span>
                  <span className="text-blue-600">{formatPrice(total)}</span>
                </div>
              </div>
            </div>
          </div>

          <div className="space-y-4">
            <h3 className="font-semibold">客户信息</h3>
            
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                收货人姓名 *
              </label>
              <input
                type="text"
                name="customerName"
                value={formData.customerName}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                联系电话 *
              </label>
              <input
                type="tel"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                收货地址 *
              </label>
              <textarea
                name="shippingAddress"
                value={formData.shippingAddress}
                onChange={handleInputChange}
                required
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              支付方式
            </label>
            <select
              name="paymentMethod"
              value={formData.paymentMethod}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="CreditCard">信用卡</option>
              <option value="DebitCard">借记卡</option>
              <option value="PayPal">PayPal</option>
              <option value="Alipay">支付宝</option>
              <option value="WeChatPay">微信支付</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              备注
            </label>
            <textarea
              name="notes"
              value={formData.notes}
              onChange={handleInputChange}
              rows={2}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="订单备注（可选）"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 text-white py-3 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50"
          >
            {loading ? '处理中...' : `确认支付 ${formatPrice(total)}`}
          </button>
        </form>
      </div>
    </div>
  );
};

export default CheckoutModal;
