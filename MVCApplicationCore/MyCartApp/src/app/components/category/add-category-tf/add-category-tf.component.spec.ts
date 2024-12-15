import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddCategoryTfComponent } from './add-category-tf.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, NgForm } from '@angular/forms';
import { CategoryService } from 'src/app/services/category.service';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('AddCategoryTfComponent', () => {
  let component: AddCategoryTfComponent;
  let fixture: ComponentFixture<AddCategoryTfComponent>;
  let categoryServiceSpy: jasmine.SpyObj<CategoryService>;
  let routerSpy: jasmine.SpyObj<Router>;
  beforeEach(() => {
    categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['AddCategory']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, RouterTestingModule],
      declarations: [AddCategoryTfComponent],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    });
    fixture = TestBed.createComponent(AddCategoryTfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should navigate to /categories on successful category addition', () => {
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };
    categoryServiceSpy.AddCategory.and.returnValue(of(mockResponse));

    const form = <NgForm><unknown>{
      valid: true,
      value: {
        categoryName: 'Test Category',
        categoryDescription: 'Test Description'
      },
      controls: {
        categoryName: { value: 'Test Category' },
        categoryDescription: { value: 'Test Description' }
      }
    };

    // Act
    component.onSubmit(form);

    // Assert
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/categories']);
    expect(component.loading).toBe(false);
  });

  it('should alert error message on unsuccessful category addition', () => {
    // Arrange
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error adding category' };
    categoryServiceSpy.AddCategory.and.returnValue(of(mockResponse));
    spyOn(window, 'alert');
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        categoryName: 'Test Category',
        categoryDescription: 'Test Description'
      },
      controls: {
        categoryName: { value: 'Test Category' },
        categoryDescription: { value: 'Test Description' }
      }
    };

    // Act
    component.onSubmit(form);

    // Assert
    expect(window.alert).toHaveBeenCalledWith('Error adding category');
    expect(component.loading).toBe(false);
  });

  it('should alert error message on HTTP error', () => {
    spyOn(window, 'alert');
    const mockError = { error: { message: 'HTTP error' } };
    categoryServiceSpy.AddCategory.and.returnValue(throwError(mockError));

    const form = <NgForm><unknown>{
      valid: true,
      value: {
        categoryName: 'Test Category',
        categoryDescription: 'Test Description'
      },
      controls: {
        categoryName: { value: 'Test Category' },
        categoryDescription: { value: 'Test Description' }
      }
    };

    component.onSubmit(form);

    expect(window.alert).toHaveBeenCalledWith('HTTP error');
    expect(component.loading).toBe(false);
  });

  it('should not call categoryService.AddCategory on invalid form submission', () => {
    const form = <NgForm>{ valid: false };

    component.onSubmit(form);

    expect(categoryServiceSpy.AddCategory).not.toHaveBeenCalled();
    expect(component.loading).toBe(false);
  });
});