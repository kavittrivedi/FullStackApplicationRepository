import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Product } from '../models/product.model';
import { AddProduct } from '../models/add-product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'http://localhost:5144/api/Product/';

  constructor(private http: HttpClient) { }
  getAllProducts(): Observable<ApiResponse<Product[]>> {
    return this.http.get<ApiResponse<Product[]>>(this.apiUrl + 'GetAllProducts');
  }

  addProduct(product: AddProduct): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.apiUrl + "AddProduct", product);
  }
}
