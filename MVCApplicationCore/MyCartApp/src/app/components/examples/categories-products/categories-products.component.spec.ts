import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriesProductsComponent } from './categories-products.component';
import { CategoryListComponent } from '../../category/category-list/category-list.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

describe('CategoriesProductsComponent', () => {
  let component: CategoriesProductsComponent;
  let fixture: ComponentFixture<CategoriesProductsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [CategoriesProductsComponent, CategoryListComponent],
    });
    fixture = TestBed.createComponent(CategoriesProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
