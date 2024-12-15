import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Category } from 'src/app/models/category.model.';
import { CategoryService } from 'src/app/services/category.service';

@Component({
  selector: 'app-update-category-tf',
  templateUrl: './update-category-tf.component.html',
  styleUrls: ['./update-category-tf.component.css']
})
export class UpdateCategoryTfComponent implements OnInit {
  categoryId: number | undefined;
  category = {
    categoryId: 0,
    categoryName: '',
    categoryDescription: ''
  };
  loading: boolean = false;

  constructor(private categoryService: CategoryService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.categoryId = +params['id'];
      this.loadCategoryDetail(this.categoryId);
    });
  }

  loadCategoryDetail(id: number): void {
    this.categoryService.getCategorById(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.category = response.data;
        } else {
          console.error('Failed to fetch category: ', response.message);
        }
      },
      error: (err) => {
        alert(err.error.message);
      },
      complete: () => {
        console.log('Completed');
      }
    });
  }

  onSubmit(updateCategoryTFForm: NgForm): void {
    if (updateCategoryTFForm.valid) {
      this.loading = true;
      console.log(updateCategoryTFForm.value);
      let updatedCategory: Category = {
        categoryId: updateCategoryTFForm.controls['categoryId'].value,
        categoryName: updateCategoryTFForm.controls['categoryName'].value,
        categoryDescription: updateCategoryTFForm.controls['categoryDescription'].value,
      };
      this.categoryService.updateCategory(updatedCategory).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/categories']);
          } else {
            alert(response.message);
          }
          this.loading = false;
        },
        error: (err) => {
          console.log(err.error.message);
          this.loading = false;
          alert(err.error.message);
        },
        complete: () => {
          this.loading = false;
          console.log("completed");
        }
      });
    }
  }
}