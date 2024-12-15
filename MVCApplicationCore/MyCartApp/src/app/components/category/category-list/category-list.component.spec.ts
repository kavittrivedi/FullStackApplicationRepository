import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoryListComponent } from './category-list.component';
import { CategoryService } from 'src/app/services/category.service';
import { Router } from '@angular/router';
import { Category } from 'src/app/models/category.model.';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('CategoryListComponent', () => {
  let component: CategoryListComponent;
  let fixture: ComponentFixture<CategoryListComponent>;
  let categoryServiceSpy: jasmine.SpyObj<CategoryService>;
  let router: Router;

  const mockCategories: Category[] = [
    { categoryId: 1, categoryName: 'Category 1', categoryDescription: 'Description 1' },
    { categoryId: 2, categoryName: 'Category 2', categoryDescription: 'Description 2' },
  ];
  
  beforeEach(() => {
    categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['getAllCategories', 'deleteCategorById']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([])],
      declarations: [CategoryListComponent],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy },
      ],
    });

    fixture = TestBed.createComponent(CategoryListComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to category details when categoryDetails is called', () => {
    // Arrange
    spyOn(router, 'navigate');

    // Act
    component.categoryDetails(1);

    // Assert
    expect(router.navigate).toHaveBeenCalledWith(['/categorydetails', 1]);
  });

  it('should navigate to update category details when updateCategoryDetails is called', () => {
    // Arrange
    spyOn(router, 'navigate');

    // Act
    component.updateCategoryDetails(1);

    // Assert
    expect(router.navigate).toHaveBeenCalledWith(['/updatecategorydetails', 1]);
  });

  it('should call confirmDelete and set categoryId for deletion', () => {
    // Arrange
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(component, 'deleteCategory');

    // Act
    component.confirmDelete(1);

    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure?');
    expect(component.categoryId).toBe(1);
    expect(component.deleteCategory).toHaveBeenCalled();
  });

  it('should not call deleteCategory if confirm is cancelled', () => {
    // Arrange
    spyOn(window, 'confirm').and.returnValue(false);
    spyOn(component, 'deleteCategory');

    // Act
    component.confirmDelete(1);

    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure?');
    expect(component.deleteCategory).not.toHaveBeenCalled();
  });

  it('should delete category and reload categories', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: true, data: "", message: 'Category deleted successfully' };
    categoryServiceSpy.deleteCategorById.and.returnValue(of(mockDeleteResponse));
    spyOn(component, 'loadCategories');

    // Act
    component.categoryId = 1;
    component.deleteCategory();

    // Assert
    expect(categoryServiceSpy.deleteCategorById).toHaveBeenCalledWith(1);
    expect(component.loadCategories).toHaveBeenCalled();
  });

  it('should alert error message if delete category fails', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: false, data: "", message: 'Failed to delete category' };
    categoryServiceSpy.deleteCategorById.and.returnValue(of(mockDeleteResponse));
    spyOn(window, 'alert');

    // Act
    component.categoryId = 1;
    component.deleteCategory();

    // Assert
    expect(window.alert).toHaveBeenCalledWith('Failed to delete category');
  });

  it('should alert error message if delete category throws error', () => {
    // Arrange
    const mockError = { error: { message: 'Delete error' } };
    categoryServiceSpy.deleteCategorById.and.returnValue(throwError(() => mockError));
    spyOn(window, 'alert');

    // Act
    component.categoryId = 1;
    component.deleteCategory();

    // Assert
    expect(window.alert).toHaveBeenCalledWith('Delete error');
  });

  it('should load categories on init', () => {
    // Arrange
    const mockResponse: ApiResponse<Category[]> = { success: true, data: mockCategories, message: '' };
    categoryServiceSpy.getAllCategories.and.returnValue(of(mockResponse));

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(categoryServiceSpy.getAllCategories).toHaveBeenCalled();
    expect(component.categories).toEqual(mockCategories);
    expect(component.loading).toBeFalse();
  });

  it('should handle failed category loading', () => {
    // Arrange
    const mockResponse: ApiResponse<Category[]> = { success: false, data: [], message: 'Failed to fetch categories' };
    categoryServiceSpy.getAllCategories.and.returnValue(of(mockResponse));
    spyOn(console, 'error');

    // Act
    fixture.detectChanges();

    // Assert
    expect(categoryServiceSpy.getAllCategories).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch categories', 'Failed to fetch categories');
    expect(component.loading).toBeFalse();
  });

  it('should handle error during category loading', () => {
    // Arrange
    const mockError = { message: 'Network error' };
    categoryServiceSpy.getAllCategories.and.returnValue(throwError(() => mockError));
    spyOn(console, 'error');

    // Act
    fixture.detectChanges();

    // Assert
    expect(categoryServiceSpy.getAllCategories).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Error fetching categories: ', mockError);
    expect(component.loading).toBeFalse();
  });

  
});
