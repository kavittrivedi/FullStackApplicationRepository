import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AddCategory } from 'src/app/models/add-category.model';
import { CategoryService } from 'src/app/services/category.service';

@Component({
  selector: 'app-add-category-tf',
  templateUrl: './add-category-tf.component.html',
  styleUrls: ['./add-category-tf.component.css']
})
export class AddCategoryTfComponent {
  category = {
    categoryName: '',
    categoryDescription: ''
  };

  loading: boolean = false;
  constructor(private categoryService: CategoryService, private router: Router) { }

  onSubmit(addCategoryTFForm: NgForm) {
    if (addCategoryTFForm.valid) {
      this.loading = true;
      console.log(addCategoryTFForm.value);
      let addCategory: AddCategory = {
        categoryName: addCategoryTFForm.controls['categoryName'].value,
        categoryDescription: addCategoryTFForm.controls['categoryDescription'].value,
      };
      this.categoryService.AddCategory(addCategory).subscribe({
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