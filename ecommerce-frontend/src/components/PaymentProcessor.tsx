import React, { useState, useEffect } from 'react';
import { PaymentRequest, PaymentResult, PaymentMethod } from '../interfaces/PaymentModels';
import paymentService from '../services/paymentService';
import { toast } from 'react-hot-toast';

interface PaymentProcessorProps {
  orderId: string;
  amount: number;
  currency?: string;
  onPaymentSuccess?: (result: PaymentResult) => void;
  onPaymentFailure?: (error: string) => void;
  onCancel?: () => void;
}

const PaymentProcessor: React.FC<PaymentProcessorProps> = ({
  orderId,
  amount,
  currency = 'USD',
  onPaymentSuccess,
  onPaymentFailure,
  onCancel
}) => {
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>(PaymentMethod.CreditCard);
  const [customerEmail, setCustomerEmail] = useState('');
  const [customerName, setCustomerName] = useState('');
  const [billingAddress, setBillingAddress] = useState('');
  const [isProcessing, setIsProcessing] = useState(false);
  const [availableMethods, setAvailableMethods] = useState<PaymentMethod[]>([]);

  useEffect(() => {
    loadPaymentMethods();
  }, []);

  const loadPaymentMethods = async () => {
    try {
      const methods = await paymentService.getPaymentMethods();
      setAvailableMethods(methods);
    } catch (error) {
      console.error('Failed to load payment methods:', error);
      // Fallback to default methods
      setAvailableMethods([
        PaymentMethod.CreditCard,
        PaymentMethod.DebitCard,
        PaymentMethod.PayPal,
        PaymentMethod.BankTransfer
      ]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!customerEmail || !customerName) {
      toast.error('Please fill in all required fields');
      return;
    }

    setIsProcessing(true);
    
    try {
      const paymentRequest: PaymentRequest = {
        orderId,
        amount,
        currency,
        paymentMethod,
        customerEmail,
        customerName,
        billingAddress: billingAddress || undefined,
        additionalData: {
          timestamp: new Date().toISOString(),
          source: 'frontend'
        }
      };

      const result = await paymentService.processPayment(paymentRequest);
      
      if (result.success) {
        toast.success('Payment processed successfully!');
        onPaymentSuccess?.(result);
      } else {
        toast.error(result.message || 'Payment failed');
        onPaymentFailure?.(result.message || 'Payment failed');
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Payment processing failed';
      toast.error(errorMessage);
      onPaymentFailure?.(errorMessage);
    } finally {
      setIsProcessing(false);
    }
  };

  const getPaymentMethodIcon = (method: PaymentMethod) => {
    switch (method) {
      case PaymentMethod.CreditCard:
      case PaymentMethod.DebitCard:
        return '💳';
      case PaymentMethod.PayPal:
        return '🔵';
      case PaymentMethod.BankTransfer:
        return '🏦';
      case PaymentMethod.Cash:
        return '💵';
      default:
        return '💰';
    }
  };

  const getPaymentMethodLabel = (method: PaymentMethod) => {
    switch (method) {
      case PaymentMethod.CreditCard:
        return 'Credit Card';
      case PaymentMethod.DebitCard:
        return 'Debit Card';
      case PaymentMethod.PayPal:
        return 'PayPal';
      case PaymentMethod.BankTransfer:
        return 'Bank Transfer';
      case PaymentMethod.Cash:
        return 'Cash';
      default:
        return method;
    }
  };

  return (
    <div className="max-w-md mx-auto bg-white rounded-lg shadow-lg p-6">
      <div className="text-center mb-6">
        <h2 className="text-2xl font-bold text-gray-900">Payment Processing</h2>
        <p className="text-gray-600 mt-2">Complete your order payment</p>
      </div>

      <div className="mb-6 p-4 bg-blue-50 rounded-lg">
        <div className="flex justify-between items-center">
          <span className="text-gray-700 font-medium">Order Total:</span>
          <span className="text-2xl font-bold text-blue-600">
            {currency} {amount.toFixed(2)}
          </span>
        </div>
        <div className="text-sm text-gray-500 mt-1">Order ID: {orderId}</div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Payment Method Selection */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Payment Method *
          </label>
          <div className="grid grid-cols-2 gap-2">
            {availableMethods.map((method) => (
              <button
                key={method}
                type="button"
                onClick={() => setPaymentMethod(method)}
                className={`p-3 border rounded-lg text-center transition-colors ${
                  paymentMethod === method
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
              >
                <div className="text-xl mb-1">{getPaymentMethodIcon(method)}</div>
                <div className="text-sm font-medium">{getPaymentMethodLabel(method)}</div>
              </button>
            ))}
          </div>
        </div>

        {/* Customer Information */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Email Address *
          </label>
          <input
            type="email"
            value={customerEmail}
            onChange={(e) => setCustomerEmail(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="your@email.com"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Full Name *
          </label>
          <input
            type="text"
            value={customerName}
            onChange={(e) => setCustomerName(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="John Doe"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Billing Address
          </label>
          <textarea
            value={billingAddress}
            onChange={(e) => setBillingAddress(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="123 Main St, City, State, ZIP"
            rows={3}
          />
        </div>

        {/* Action Buttons */}
        <div className="flex space-x-3 pt-4">
          <button
            type="button"
            onClick={onCancel}
            disabled={isProcessing}
            className="flex-1 px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:border-transparent disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={isProcessing}
            className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isProcessing ? (
              <div className="flex items-center justify-center">
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Processing...
              </div>
            ) : (
              'Process Payment'
            )}
          </button>
        </div>
      </form>

      {/* Security Notice */}
      <div className="mt-6 text-center">
        <div className="flex items-center justify-center text-green-600 mb-2">
          <svg className="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clipRule="evenodd" />
          </svg>
          Secure Payment
        </div>
        <p className="text-xs text-gray-500">
          Your payment information is encrypted and secure
        </p>
      </div>
    </div>
  );
};

export default PaymentProcessor;
