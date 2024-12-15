import { Component, OnInit, ɵsetAlternateWeakRefImpl } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { AddProduct } from 'src/app/models/add-product.model';
import { Category } from 'src/app/models/category.model.';
import { CategoryService } from 'src/app/services/category.service';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent implements OnInit {
  categories: Category[] = [];
  product: AddProduct = {
    productName: '',
    productDescription: '',
    categoryId: 0,
    productPrice: 0,
    inStock: false,
    isActive: false
  };
  loading: boolean = false;


  constructor(
    private categoryService: CategoryService,
    private prodductService: ProductService,
    private router: Router
  ) { }
  ngOnInit(): void {
    this.loadCategories();
  }
  loadCategories(): void {
    this.loading = true;
    this.categoryService.getAllCategories().subscribe({
      next: (response: ApiResponse<Category[]>) => {
        if (response.success) {
          this.categories = response.data;
        } else {
          console.error('Failed to fetch categories', response.message);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching categories: ', error);
        this.loading = false;
      }
    });
  }

  onSubmit(productForm: NgForm): void {
    if (productForm.valid) {
      this.loading = true;
      this.prodductService.addProduct(this.product).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/products']);
          } else {
            alert(response.message);
          }
          this.loading = false;
        },
        error: (err) => {
          this.loading = false;
          alert(err.error.message);
        },
        complete: () => {
          this.loading = false;
          console.log('completed');
        }
      });
    }
  }
}