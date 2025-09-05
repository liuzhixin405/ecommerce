import apiClient from '../api/client';

export interface UploadResponse {
  success: boolean;
  fileName: string;
  fileUrl: string;
  fileSize: number;
  message: string;
}

export interface ImageInfo {
  fileName: string;
  fileUrl: string;
  fileSize: number;
  uploadDate: string;
}

export interface ImageListResponse {
  images: ImageInfo[];
}

export const fileUploadService = {
  // 上传产品图片
  uploadProductImage: async (file: File): Promise<UploadResponse> => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.post<UploadResponse>('/fileupload/product-image', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  },

  // 删除产品图片
  deleteProductImage: async (fileName: string): Promise<{ success: boolean; message: string }> => {
    const response = await apiClient.delete(`/fileupload/product-image/${fileName}`);
    return response.data;
  },

  // 获取所有已上传的图片列表
  getProductImages: async (): Promise<ImageListResponse> => {
    const response = await apiClient.get<ImageListResponse>('/fileupload/product-images');
    return response.data;
  }
};
