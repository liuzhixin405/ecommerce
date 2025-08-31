import React, { useState, useEffect } from 'react';
import { Product } from '../interfaces';
import { getProducts, searchProducts, getProductsByCategory } from '../services/productService';
import ProductCard from '../components/ProductCard';
import Button from '../components/ui/Button';
import { Search, Filter } from 'lucide-react';

const ProductList: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [categories, setCategories] = useState<string[]>([]);

  useEffect(() => {
    loadProducts();
  }, []);

  useEffect(() => {
    if (searchQuery) {
      handleSearch();
    } else if (selectedCategory) {
      handleCategoryFilter();
    } else {
      loadProducts();
    }
  }, [searchQuery, selectedCategory]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await getProducts();
      setProducts(data);
      
      // 提取所有分类
      const uniqueCategories = Array.from(new Set(data.map(p => p.category)));
      setCategories(uniqueCategories);
    } catch (error) {
      console.error('Failed to load products:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      const data = await searchProducts(searchQuery);
      setProducts(data);
    } catch (error) {
      console.error('Failed to search products:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCategoryFilter = async () => {
    try {
      setLoading(true);
      const data = await getProductsByCategory(selectedCategory);
      setProducts(data);
    } catch (error) {
      console.error('Failed to filter products:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleViewDetails = (product: Product) => {
    // 这里可以导航到产品详情页面
    console.log('View product details:', product);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-4">产品列表</h1>
        
        {/* 搜索和筛选 */}
        <div className="flex flex-col md:flex-row gap-4 mb-6">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="搜索产品..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          
          <div className="flex items-center gap-2">
            <Filter className="w-5 h-5 text-gray-400" />
            <select
              value={selectedCategory}
              onChange={(e) => setSelectedCategory(e.target.value)}
              className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">所有分类</option>
              {categories.map((category) => (
                <option key={category} value={category}>
                  {category}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* 结果统计 */}
        <div className="text-gray-600 mb-4">
          找到 {products.length} 个产品
        </div>
      </div>

      {/* 产品网格 */}
      {products.length === 0 ? (
        <div className="text-center py-12">
          <h3 className="text-lg font-semibold text-gray-600 mb-2">没有找到产品</h3>
          <p className="text-gray-500">尝试调整搜索条件或分类筛选</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {products.map((product) => (
            <ProductCard
              key={product.id}
              product={product}
              onViewDetails={handleViewDetails}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default ProductList;
