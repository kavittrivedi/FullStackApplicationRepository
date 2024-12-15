import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Category } from 'src/app/models/category.model.';
import { CategoryService } from 'src/app/services/category.service';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-add-product-rf',
  templateUrl: './add-product-rf.component.html',
  styleUrls: ['./add-product-rf.component.css']
})
export class AddProductRfComponent implements OnInit {
  loading: boolean = false;
  categories: Category[] = [];
  productForm!: FormGroup;
  constructor(
    private categoryService: CategoryService,
    private productService: ProductService,
    private fb: FormBuilder,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.productForm = this.fb.group({
      productName: ['', [Validators.required, Validators.minLength(2)]],
      productDescription: ['', [Validators.required, Validators.minLength(5)]],
      categoryId: [0, [Validators.required, this.categoryValidator]],
      productPrice: [0, [Validators.required, Validators.min(0.01)]],
      inStock: [, Validators.required],
      isActive: [true],
    });
    this.loadCategories();
  }

  categoryValidator(control: any) {
    return control.value == '' ? { invalidCategory: true } : null;
  }

  get formControls() {
    return this.productForm.controls;
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

  onSubmit() {
    if (this.productForm.valid) {
      console.log(this.productForm.value);
      this.productService.addProduct(this.productForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/products']);
          } else {
            alert(response.message);
          }
        },
        error: (err) => {
          console.error('Failed to add product: ', err.error.message);
        },
        complete: () => {
          console.log('Completed');
        }
      });
    }
  }
}
