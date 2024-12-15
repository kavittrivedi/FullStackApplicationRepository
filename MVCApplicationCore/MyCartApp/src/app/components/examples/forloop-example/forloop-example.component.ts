import { Component } from '@angular/core';

@Component({
  selector: 'app-forloop-example',
  templateUrl: './forloop-example.component.html',
  styleUrls: ['./forloop-example.component.css']
})
export class ForloopExampleComponent {
  Categories = [
    { categoryId: 1, categoryName: 'Category 1', categoryDescription: 'Description 1' },
    { categoryId: 2, categoryName: 'Category 2', categoryDescription: 'Description 2' }
  ];
}
