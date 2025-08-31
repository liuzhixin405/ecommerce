export enum InventoryOperationType {
  StockIn = 'StockIn',
  StockOut = 'StockOut',
  StockAdjustment = 'StockAdjustment',
  StockLock = 'StockLock',
  StockRelease = 'StockRelease',
  StockReservation = 'StockReservation'
}

export interface InventoryCheckResult {
  isAvailable: boolean;
  productId: string;
  requestedQuantity: number;
  availableStock: number;
  lockedStock: number;
  reservedStock: number;
  message: string;
  checkedAt: Date;
}

export interface InventoryOperationResult {
  success: boolean;
  productId: string;
  quantity: number;
  oldStock: number;
  newStock: number;
  operationType: InventoryOperationType;
  message: string;
  operationTime: Date;
  orderId?: string;
}

export interface ProductInventoryInfo {
  productId: string;
  productName: string;
  currentStock: number;
  lockedStock: number;
  reservedStock: number;
  availableStock: number;
  lastUpdated: Date;
  lowStockThreshold: number;
  isLowStock: boolean;
}

export interface InventoryUpdate {
  productId: string;
  quantity: number;
  operationType: InventoryOperationType;
  orderId?: string;
  notes?: string;
}

export interface BatchInventoryUpdateResult {
  success: boolean;
  totalOperations: number;
  successfulOperations: number;
  failedOperations: number;
  results: InventoryOperationResult[];
  message: string;
  processedAt: Date;
}

export interface InventoryTransaction {
  id: string;
  productId: string;
  operationType: InventoryOperationType;
  quantity: number;
  oldStock: number;
  newStock: number;
  orderId?: string;
  userId?: string;
  notes?: string;
  createdAt: Date;
}
