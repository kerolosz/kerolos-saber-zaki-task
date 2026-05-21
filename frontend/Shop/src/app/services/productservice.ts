import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiConstants } from '../core/constants/ApiConstants';
import { ApiResponse } from '../models/Response';
import { Product } from '../models/Product';

@Injectable({
  providedIn: 'root',
})
export class Productservice {
  constructor(private http: HttpClient) {}
  getProducts() {
    return this.http.get<ApiResponse<Product[]>>(ApiConstants.Base_URL + 'Product/GetAllProducts');
  }
  getProductByID(id: number) {
    return this.http.get<ApiResponse<Product>>(
      ApiConstants.Base_URL + 'Product/GetProductById?id=' + id,
    );
  }
  deleteProduct(id: number) {
    return this.http.delete(ApiConstants.Base_URL + 'Product/DeleteProduct/' + id);
  }
  createProduct(formData: FormData) {
    return this.http.post(ApiConstants.Base_URL + 'Product', formData);
  }
  updateProduct(formData: FormData) {
    return this.http.put(ApiConstants.Base_URL + 'Product', formData);
  }
}
