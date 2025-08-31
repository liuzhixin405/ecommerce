import React, { useState, useEffect } from 'react';
import { 
  ProductInventoryInfo, 
  InventoryOperationType, 
  InventoryUpdate,
  InventoryOperationResult 
} from '../interfaces/InventoryModels';
import inventoryService from '../services/inventoryService';
import { toast } from 'react-hot-toast';

interface InventoryManagerProps {
  productId: string;
  productName: string;
  onInventoryUpdate?: (result: InventoryOperationResult) => void;
}

const InventoryManager: React.FC<InventoryManagerProps> = ({
  productId,
  productName,
  onInventoryUpdate
}) => {
  const [inventoryInfo, setInventoryInfo] = useState<ProductInventoryInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isUpdating, setIsUpdating] = useState(false);
  const [operationType, setOperationType] = useState<InventoryOperationType>(InventoryOperationType.StockAdjustment);
  const [quantity, setQuantity] = useState<number>(0);
  const [notes, setNotes] = useState('');

  useEffect(() => {
    loadInventoryInfo();
  }, [productId]);

  const loadInventoryInfo = async () => {
    try {
      setIsLoading(true);
      const info = await inventoryService.getProductInventory(productId);
      setInventoryInfo(info);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load inventory info';
      toast.error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const handleInventoryUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (quantity === 0) {
      toast.error('Please enter a valid quantity');
      return;
    }

    setIsUpdating(true);
    
    try {
      const update: InventoryUpdate = {
        productId,
        quantity: Math.abs(quantity),
        operationType,
        notes: notes || undefined
      };

      const result = await inventoryService.batchUpdateInventory([update]);
      
      if (result.success) {
        toast.success('Inventory updated successfully!');
        setQuantity(0);
        setNotes('');
        onInventoryUpdate?.(result.results[0]);
        loadInventoryInfo(); // Refresh inventory info
      } else {
        toast.error(result.message || 'Inventory update failed');
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Inventory update failed';
      toast.error(errorMessage);
    } finally {
      setIsUpdating(false);
    }
  };

  const getOperationTypeLabel = (type: InventoryOperationType) => {
    switch (type) {
      case InventoryOperationType.StockIn:
        return 'Stock In (+ Add)';
      case InventoryOperationType.StockOut:
        return 'Stock Out (- Remove)';
      case InventoryOperationType.StockAdjustment:
        return 'Stock Adjustment';
      case InventoryOperationType.StockLock:
        return 'Stock Lock';
      case InventoryOperationType.StockRelease:
        return 'Stock Release';
      case InventoryOperationType.StockReservation:
        return 'Stock Reservation';
      default:
        return type;
    }
  };

  const getOperationTypeIcon = (type: InventoryOperationType) => {
    switch (type) {
      case InventoryOperationType.StockIn:
        return '📥';
      case InventoryOperationType.StockOut:
        return '📤';
      case InventoryOperationType.StockAdjustment:
        return '⚖️';
      case InventoryOperationType.StockLock:
        return '🔒';
      case InventoryOperationType.StockRelease:
        return '🔓';
      case InventoryOperationType.StockReservation:
        return '📋';
      default:
        return '📊';
    }
  };

  if (isLoading) {
    return (
      <div className="bg-white rounded-lg shadow p-6">
        <div className="animate-pulse">
          <div className="h-4 bg-gray-200 rounded w-1/4 mb-4"></div>
          <div className="space-y-3">
            <div className="h-4 bg-gray-200 rounded"></div>
            <div className="h-4 bg-gray-200 rounded w-5/6"></div>
          </div>
        </div>
      </div>
    );
  }

  if (!inventoryInfo) {
    return (
      <div className="bg-white rounded-lg shadow p-6">
        <div className="text-center text-gray-500">
          Failed to load inventory information
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-lg p-6">
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-2">Inventory Management</h3>
        <p className="text-sm text-gray-600">{productName}</p>
      </div>

      {/* Current Inventory Status */}
      <div className="grid grid-cols-2 gap-4 mb-6">
        <div className="bg-blue-50 p-4 rounded-lg">
          <div className="text-2xl font-bold text-blue-600">{inventoryInfo.currentStock}</div>
          <div className="text-sm text-blue-700">Current Stock</div>
        </div>
        <div className="bg-green-50 p-4 rounded-lg">
          <div className="text-2xl font-bold text-green-600">{inventoryInfo.availableStock}</div>
          <div className="text-sm text-green-700">Available Stock</div>
        </div>
        <div className="bg-yellow-50 p-4 rounded-lg">
          <div className="text-2xl font-bold text-yellow-600">{inventoryInfo.lockedStock}</div>
          <div className="text-sm text-yellow-700">Locked Stock</div>
        </div>
        <div className="bg-purple-50 p-4 rounded-lg">
          <div className="text-2xl font-bold text-purple-600">{inventoryInfo.reservedStock}</div>
          <div className="text-sm text-purple-700">Reserved Stock</div>
        </div>
      </div>

      {/* Low Stock Warning */}
      {inventoryInfo.isLowStock && (
        <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
          <div className="flex items-center">
            <svg className="w-5 h-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
            </svg>
            <span className="text-red-800 font-medium">Low Stock Warning</span>
          </div>
          <p className="text-red-700 text-sm mt-1">
            Current stock ({inventoryInfo.currentStock}) is below the threshold ({inventoryInfo.lowStockThreshold})
          </p>
        </div>
      )}

      {/* Inventory Update Form */}
      <form onSubmit={handleInventoryUpdate} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Operation Type
          </label>
          <select
            value={operationType}
            onChange={(e) => setOperationType(e.target.value as InventoryOperationType)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            {Object.values(InventoryOperationType).map((type) => (
              <option key={type} value={type}>
                {getOperationTypeIcon(type)} {getOperationTypeLabel(type)}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Quantity
          </label>
          <input
            type="number"
            value={quantity}
            onChange={(e) => setQuantity(Number(e.target.value))}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Enter quantity"
            min="1"
            required
          />
          <p className="text-xs text-gray-500 mt-1">
            {operationType === InventoryOperationType.StockIn && 'Positive number to add stock'}
            {operationType === InventoryOperationType.StockOut && 'Positive number to remove stock'}
            {operationType === InventoryOperationType.StockAdjustment && 'Positive or negative number to adjust stock'}
          </p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Notes (Optional)
          </label>
          <textarea
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Add notes about this operation..."
            rows={3}
          />
        </div>

        <button
          type="submit"
          disabled={isUpdating || quantity === 0}
          className="w-full px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isUpdating ? (
            <div className="flex items-center justify-center">
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
              Updating...
            </div>
          ) : (
            'Update Inventory'
          )}
        </button>
      </form>

      {/* Last Updated Info */}
      <div className="mt-6 pt-4 border-t border-gray-200">
        <div className="text-sm text-gray-500">
          Last updated: {new Date(inventoryInfo.lastUpdated).toLocaleString()}
        </div>
      </div>
    </div>
  );
};

export default InventoryManager;
