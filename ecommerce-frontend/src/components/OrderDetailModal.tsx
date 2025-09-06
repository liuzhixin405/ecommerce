import React from 'react';
import { Order } from '../interfaces';
import { X, Package, MapPin, Phone, CreditCard, Calendar, User, Truck } from 'lucide-react';
import { formatPrice } from '../utils/format';

interface OrderDetailModalProps {
  order: Order | null;
  isOpen: boolean;
  onClose: () => void;
}

const OrderDetailModal: React.FC<OrderDetailModalProps> = ({ order, isOpen, onClose }) => {
  if (!order) return null;

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Pending':
        return <Calendar className="w-5 h-5 text-yellow-500" />;
      case 'Paid':
        return <CreditCard className="w-5 h-5 text-green-500" />;
      case 'Shipped':
        return <Truck className="w-5 h-5 text-blue-500" />;
      case 'Delivered':
        return <Package className="w-5 h-5 text-green-600" />;
      case 'Cancelled':
        return <X className="w-5 h-5 text-red-500" />;
      default:
        return <Calendar className="w-5 h-5 text-gray-500" />;
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Pending':
        return '待支付';
      case 'Paid':
        return '已支付';
      case 'Shipped':
        return '已发货';
      case 'Delivered':
        return '已送达';
      case 'Cancelled':
        return '已取消';
      default:
        return status;
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Paid':
        return 'bg-green-100 text-green-800';
      case 'Shipped':
        return 'bg-blue-100 text-blue-800';
      case 'Delivered':
        return 'bg-green-100 text-green-800';
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className={`fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 ${isOpen ? 'block' : 'hidden'}`}>
      <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        {/* 头部 */}
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-2xl font-bold text-gray-900">订单详情</h2>
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-100 rounded-full transition-colors"
          >
            <X className="w-6 h-6 text-gray-500" />
          </button>
        </div>

        {/* 内容 */}
        <div className="p-6 space-y-6">
          {/* 订单基本信息 */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-3">订单信息</h3>
                <div className="space-y-2">
                  <div className="flex items-center">
                    <span className="text-sm font-medium text-gray-600 w-20">订单号:</span>
                    <span className="text-sm text-gray-900">{order.orderNumber}</span>
                  </div>
                  <div className="flex items-center">
                    <span className="text-sm font-medium text-gray-600 w-20">创建时间:</span>
                    <span className="text-sm text-gray-900">
                      {new Date(order.createdAt).toLocaleString('zh-CN')}
                    </span>
                  </div>
                  <div className="flex items-center">
                    <span className="text-sm font-medium text-gray-600 w-20">更新时间:</span>
                    <span className="text-sm text-gray-900">
                      {new Date(order.updatedAt).toLocaleString('zh-CN')}
                    </span>
                  </div>
                  <div className="flex items-center">
                    <span className="text-sm font-medium text-gray-600 w-20">订单状态:</span>
                    <div className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(order.status)}`}>
                      {getStatusIcon(order.status)}
                      <span className="ml-1">{getStatusText(order.status)}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div className="space-y-4">
              <div>
                <h3 className="text-lg font-semibold text-gray-900 mb-3">收货信息</h3>
                <div className="space-y-2">
                  <div className="flex items-start">
                    <User className="w-4 h-4 text-gray-400 mt-0.5 mr-2" />
                    <span className="text-sm text-gray-900">{order.shippingName}</span>
                  </div>
                  <div className="flex items-start">
                    <Phone className="w-4 h-4 text-gray-400 mt-0.5 mr-2" />
                    <span className="text-sm text-gray-900">{order.shippingPhone}</span>
                  </div>
                  <div className="flex items-start">
                    <MapPin className="w-4 h-4 text-gray-400 mt-0.5 mr-2" />
                    <span className="text-sm text-gray-900">{order.shippingAddress}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* 商品列表 */}
          <div>
            <h3 className="text-lg font-semibold text-gray-900 mb-4">商品清单</h3>
            <div className="bg-gray-50 rounded-lg p-4">
              <div className="space-y-4">
                {order.items.map((item, index) => (
                  <div key={index} className="flex items-center space-x-4 p-3 bg-white rounded-lg">
                    <img
                      src={item.productImage || item.product?.imageUrl || 'https://via.placeholder.com/80x80'}
                      alt={item.productName || item.product?.name}
                      className="w-16 h-16 object-cover rounded"
                    />
                    <div className="flex-1">
                      <h4 className="font-medium text-gray-900">
                        {item.productName || item.product?.name}
                      </h4>
                      <p className="text-sm text-gray-600">
                        分类: {item.product?.category || '未知'}
                      </p>
                      <p className="text-sm text-gray-600">
                        数量: {item.quantity}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="font-medium text-gray-900">
                        ¥{formatPrice(item.price)}
                      </p>
                      <p className="text-sm text-gray-600">
                        小计: ¥{formatPrice(item.price * item.quantity)}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* 订单总计 */}
          <div className="bg-blue-50 rounded-lg p-4">
            <div className="flex justify-between items-center">
              <span className="text-lg font-semibold text-gray-900">订单总计:</span>
              <span className="text-2xl font-bold text-blue-600">
                ¥{formatPrice(order.totalAmount)}
              </span>
            </div>
          </div>

          {/* 订单操作 */}
          <div className="flex justify-end space-x-3 pt-4 border-t">
            {order.status === 'Pending' && (
              <button className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                立即支付
              </button>
            )}
            {order.status === 'Delivered' && (
              <button className="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors">
                确认收货
              </button>
            )}
            <button
              onClick={onClose}
              className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              关闭
            </button>
            {order.status === 'Pending' && (
              <button className="px-6 py-2 border border-red-300 text-red-700 rounded-lg hover:bg-red-50 transition-colors">
                取消订单
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderDetailModal;
