import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateCategoryTfComponent } from './update-category-tf.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { CategoryService } from 'src/app/services/category.service';
import { Category } from 'src/app/models/category.model.';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { FormsModule } from '@angular/forms';

describe('UpdateCategoryTfComponent', () => {
  let component: UpdateCategoryTfComponent;
  let fixture: ComponentFixture<UpdateCategoryTfComponent>;
  let categoryServiceSpy: jasmine.SpyObj<CategoryService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let route: ActivatedRoute;
  const mockCategory: Category = {
    categoryId: 1,
    categoryName: 'Test Category',
    categoryDescription: 'Test Description'
  };
  beforeEach(() => {
    categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['getCategorById']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, RouterTestingModule.withRoutes([])],
      declarations: [UpdateCategoryTfComponent],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({ id: 1 })
          }
        }
      ]
    });
    fixture = TestBed.createComponent(UpdateCategoryTfComponent);
    component = fixture.componentInstance;
    route = TestBed.inject(ActivatedRoute);
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize categoryId from route params and load category details', () => {
    // Arrange
    const mockResponse: ApiResponse<Category> = { success: true, data: mockCategory, message: '' };
    categoryServiceSpy.getCategorById.and.returnValue(of(mockResponse));

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(component.categoryId).toBe(1);
    expect(categoryServiceSpy.getCategorById).toHaveBeenCalledWith(1);
    expect(component.category).toEqual(mockCategory);
  });

  it('should log error message if category loading fails', () => {
    // Arrange
    const mockResponse: ApiResponse<Category> = { success: false, data: mockCategory, message: 'Failed to fetch category' };
    categoryServiceSpy.getCategorById.and.returnValue(of(mockResponse));
    spyOn(console, 'error');

    // Act
    fixture.detectChanges();

    // Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch category: ', 'Failed to fetch category');
  });

  it('should alert error message on HTTP error', () => {
    // Arrange
    spyOn(window, 'alert');
    const mockError = { error: { message: 'HTTP error' } };
    categoryServiceSpy.getCategorById.and.returnValue(throwError(mockError));

    // Act
    fixture.detectChanges();

    // Assert
    expect(window.alert).toHaveBeenCalledWith('HTTP error');
  });

  it('should log "Completed" when category loading completes', () => {
    // Arrange
    const mockResponse: ApiResponse<Category> = { success: true, data: mockCategory, message: '' };
    categoryServiceSpy.getCategorById.and.returnValue(of(mockResponse));
    spyOn(console, 'log');

    // Act
    fixture.detectChanges();

    // Assert
    expect(console.log).toHaveBeenCalledWith('Completed');
  });
});
