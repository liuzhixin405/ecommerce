import React from 'react';
import { Product } from '../interfaces';
import { formatPrice } from '../utils/format';
import { cartService } from '../services/cartService';
import Button from './ui/Button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from './ui/Card';
import { ShoppingCart, Eye } from 'lucide-react';

interface ProductCardProps {
  product: Product;
  onViewDetails?: (product: Product) => void;
}

const ProductCard: React.FC<ProductCardProps> = ({ product, onViewDetails }) => {
  const handleAddToCart = () => {
    cartService.addToCart(product, 1);
    // 这里可以添加成功提示
  };

  const handleViewDetails = () => {
    if (onViewDetails) {
      onViewDetails(product);
    }
  };

  return (
    <Card className="h-full flex flex-col">
      <div className="aspect-square overflow-hidden rounded-t-lg">
        <img
          src={product.imageUrl || 'https://via.placeholder.com/300x300?text=No+Image'}
          alt={product.name}
          className="w-full h-full object-cover hover:scale-105 transition-transform duration-300"
        />
      </div>
      
      <CardHeader className="flex-1">
        <CardTitle className="text-lg font-semibold line-clamp-2">
          {product.name}
        </CardTitle>
        <p className="text-sm text-gray-600 line-clamp-2">
          {product.description}
        </p>
        <div className="flex items-center justify-between mt-2">
          <span className="text-lg font-bold text-blue-600">
            {formatPrice(product.price)}
          </span>
          <span className="text-sm text-gray-500">
            库存: {product.stock}
          </span>
        </div>
      </CardHeader>

      <CardFooter className="flex gap-2">
        <Button
          variant="outline"
          size="sm"
          className="flex-1"
          onClick={handleViewDetails}
        >
          <Eye className="w-4 h-4 mr-1" />
          查看详情
        </Button>
        <Button
          size="sm"
          className="flex-1"
          onClick={handleAddToCart}
          disabled={product.stock === 0}
        >
          <ShoppingCart className="w-4 h-4 mr-1" />
          {product.stock === 0 ? '缺货' : '加入购物车'}
        </Button>
      </CardFooter>
    </Card>
  );
};

export default ProductCard;
