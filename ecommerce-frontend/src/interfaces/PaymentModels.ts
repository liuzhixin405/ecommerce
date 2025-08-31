export enum PaymentState {
  Pending = 'Pending',
  Processing = 'Processing',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

export enum RefundStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Completed = 'Completed'
}

export enum PaymentMethod {
  CreditCard = 'CreditCard',
  DebitCard = 'DebitCard',
  PayPal = 'PayPal',
  BankTransfer = 'BankTransfer',
  Cash = 'Cash'
}

export interface PaymentRequest {
  orderId: string;
  amount: number;
  currency: string;
  paymentMethod: PaymentMethod;
  customerEmail?: string;
  customerName?: string;
  billingAddress?: string;
  additionalData?: Record<string, string>;
}

export interface PaymentResult {
  success: boolean;
  paymentId: string;
  transactionId: string;
  status: PaymentStatus;
  message: string;
  processedAt: Date;
  additionalData?: Record<string, string>;
}

export interface PaymentStatus {
  paymentId: string;
  orderId: string;
  state: PaymentState;
  amount: number;
  currency: string;
  paymentMethod: PaymentMethod;
  createdAt: Date;
  processedAt?: Date;
  message?: string;
}

export interface PaymentValidationResult {
  isValid: boolean;
  paymentId: string;
  orderId: string;
  state: PaymentState;
  message: string;
  validatedAt: Date;
}

export interface RefundRequest {
  orderId: string;
  paymentId: string;
  amount: number;
  reason: string;
  customerEmail?: string;
}

export interface RefundResult {
  success: boolean;
  refundId: string;
  paymentId: string;
  orderId: string;
  amount: number;
  status: RefundStatus;
  message: string;
  processedAt: Date;
}
