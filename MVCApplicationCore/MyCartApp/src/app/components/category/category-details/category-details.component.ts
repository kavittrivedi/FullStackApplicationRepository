import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Category } from 'src/app/models/category.model.';
import { CategoryService } from 'src/app/services/category.service';

@Component({
  selector: 'app-category-details',
  templateUrl: './category-details.component.html',
  styleUrls: ['./category-details.component.css']
})
export class CategoryDetailsComponent implements OnInit {
categoryId! : number ;
  category!: Category;
  constructor(private categoryService: CategoryService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) =>{
      this.categoryId = params['id'];
      this.loadCategoryDetail(this.categoryId);
    });
  }

  loadCategoryDetail(id: number): void{
    this.categoryService.getCategorById(id).subscribe({
      next:(response) =>{
        if(response.success){
          this.category = response.data;
        }else{
          console.error('Failed to fetch category: ', response.message);
        }
      },
      error: (err)=>{
        alert(err.error.message);
      },
      complete:()=>{
        console.log('Completed');
      }
    });
  }
}
