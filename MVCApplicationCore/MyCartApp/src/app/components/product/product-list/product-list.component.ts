import { Component, OnInit } from '@angular/core';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Product } from 'src/app/models/product.model';
import { ProductService } from 'src/app/services/product.service';


@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[] | undefined;
  loading: boolean = false;
  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getAllProducts().subscribe({
      next: (response: ApiResponse<Product[]>) => {
        if (response.success) {
          this.products = response.data;
        } else {
          console.error('Failed to fetch products: ', response.message);
        }
        this.loading = false;
      },
      error:(error) =>{
        console.error('Error fetching products:', error);
        this.loading= false;
      }
    });
  }
}
