import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { getOrders } from '../services/orderService';
import { Order } from '../interfaces';
import { Calendar, Package, CreditCard, Truck, CheckCircle, XCircle, Clock } from 'lucide-react';
import OrderDetailModal from '../components/OrderDetailModal';

const OrdersPage: React.FC = () => {
  const { isAuthenticated, user } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedStatus, setSelectedStatus] = useState<string>('');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);

  useEffect(() => {
    if (isAuthenticated) {
      loadOrders();
    }
  }, [isAuthenticated]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await getOrders();
      setOrders(data);
    } catch (error) {
      console.error('Failed to load orders:', error);
    } finally {
      setLoading(false);
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'Pending':
        return <Clock className="w-5 h-5 text-yellow-500" />;
      case 'Paid':
        return <CheckCircle className="w-5 h-5 text-green-500" />;
      case 'Shipped':
        return <Truck className="w-5 h-5 text-blue-500" />;
      case 'Delivered':
        return <Package className="w-5 h-5 text-green-600" />;
      case 'Cancelled':
        return <XCircle className="w-5 h-5 text-red-500" />;
      default:
        return <Clock className="w-5 h-5 text-gray-500" />;
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

  const filteredOrders = selectedStatus 
    ? orders.filter(order => order.status === selectedStatus)
    : orders;

  const handleViewOrderDetails = (order: Order) => {
    setSelectedOrder(order);
    setIsDetailModalOpen(true);
  };

  const handleCloseDetailModal = () => {
    setSelectedOrder(null);
    setIsDetailModalOpen(false);
  };

  if (!isAuthenticated) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center py-12">
          <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">请先登录</h2>
          <p className="text-gray-600">登录后即可查看您的订单</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">我的订单</h1>
        
        {/* 状态筛选 */}
        <div className="flex flex-wrap gap-2 mb-6">
          <button
            onClick={() => setSelectedStatus('')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === '' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            全部订单
          </button>
          <button
            onClick={() => setSelectedStatus('Pending')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Pending' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            待支付
          </button>
          <button
            onClick={() => setSelectedStatus('Paid')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Paid' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已支付
          </button>
          <button
            onClick={() => setSelectedStatus('Shipped')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Shipped' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已发货
          </button>
          <button
            onClick={() => setSelectedStatus('Delivered')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              selectedStatus === 'Delivered' 
                ? 'bg-blue-600 text-white' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            已送达
          </button>
        </div>
      </div>

      {loading ? (
        <div className="space-y-4">
          {[...Array(3)].map((_, index) => (
            <div key={index} className="bg-white rounded-lg shadow-md p-6 animate-pulse">
              <div className="flex justify-between items-start mb-4">
                <div className="bg-gray-200 h-6 w-32 rounded"></div>
                <div className="bg-gray-200 h-6 w-20 rounded"></div>
              </div>
              <div className="bg-gray-200 h-4 w-48 rounded mb-2"></div>
              <div className="bg-gray-200 h-4 w-32 rounded"></div>
            </div>
          ))}
        </div>
      ) : filteredOrders.length === 0 ? (
        <div className="text-center py-12">
          <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            {selectedStatus ? `没有${getStatusText(selectedStatus)}的订单` : '暂无订单'}
          </h2>
          <p className="text-gray-600">
            {selectedStatus ? '尝试选择其他状态查看订单' : '快去选购心仪的商品吧'}
          </p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredOrders.map((order) => (
            <div key={order.id} className="bg-white rounded-lg shadow-md p-6">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">
                    订单号: {order.orderNumber}
                  </h3>
                  <div className="flex items-center mt-1 text-sm text-gray-600">
                    <Calendar className="w-4 h-4 mr-1" />
                    {new Date(order.createdAt).toLocaleDateString('zh-CN')}
                  </div>
                </div>
                <div className={`flex items-center px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
                  {getStatusIcon(order.status)}
                  <span className="ml-1">{getStatusText(order.status)}</span>
                </div>
              </div>

              <div className="border-t pt-4">
                <div className="flex justify-between items-center mb-4">
                  <div className="text-sm text-gray-600">
                    共 {order.items.length} 件商品
                  </div>
                  <div className="text-lg font-semibold text-gray-900">
                    ¥{order.totalAmount.toFixed(2)}
                  </div>
                </div>

                {/* 订单商品列表 */}
                <div className="space-y-3">
                  {order.items.map((item, index) => (
                    <div key={index} className="flex items-center space-x-4 p-3 bg-gray-50 rounded-lg">
                      <img
                        src={item.productImage || 'https://via.placeholder.com/60x60'}
                        alt={item.productName}
                        className="w-15 h-15 object-cover rounded"
                      />
                      <div className="flex-1">
                        <h4 className="font-medium text-gray-900">{item.productName}</h4>
                        <p className="text-sm text-gray-600">数量: {item.quantity}</p>
                      </div>
                      <div className="text-right">
                        <p className="font-medium text-gray-900">¥{item.price.toFixed(2)}</p>
                      </div>
                    </div>
                  ))}
                </div>

                {/* 订单操作按钮 */}
                <div className="flex justify-end space-x-3 mt-4 pt-4 border-t">
                  {order.status === 'Pending' && (
                    <button className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                      立即支付
                    </button>
                  )}
                  {order.status === 'Delivered' && (
                    <button className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors">
                      确认收货
                    </button>
                  )}
                  <button 
                    onClick={() => handleViewOrderDetails(order)}
                    className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                  >
                    查看详情
                  </button>
                  {order.status === 'Pending' && (
                    <button className="px-4 py-2 border border-red-300 text-red-700 rounded-lg hover:bg-red-50 transition-colors">
                      取消订单
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* 订单详情模态框 */}
      <OrderDetailModal
        order={selectedOrder}
        isOpen={isDetailModalOpen}
        onClose={handleCloseDetailModal}
      />
    </div>
  );
};

export default OrdersPage;
