import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoryDetailsComponent } from './category-details.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { CategoryService } from 'src/app/services/category.service';
import { Category } from 'src/app/models/category.model.';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';

describe('CategoryDetailsComponent', () => {
  let component: CategoryDetailsComponent;
  let fixture: ComponentFixture<CategoryDetailsComponent>;
  let categoryService: jasmine.SpyObj<CategoryService>;
  let route: ActivatedRoute;
  const mockCategory: Category = {
    categoryId: 1, categoryName: 'Test Category',
    categoryDescription: 'Test description'
  };
  beforeEach(() => {
    const categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['getCategorById']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([])],
      declarations: [CategoryDetailsComponent],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({ id: 1 })
          }
        }
      ]
    });
    fixture = TestBed.createComponent(CategoryDetailsComponent);
    component = fixture.componentInstance;
    categoryService = TestBed.inject(CategoryService) as jasmine.SpyObj<CategoryService>;
    route = TestBed.inject(ActivatedRoute);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize categoryId from route params and load category details', () => {
    // Arrange
    const mockResponse: ApiResponse<Category> = { success: true, data: mockCategory, message: '' };
    categoryService.getCategorById.and.returnValue(of(mockResponse));

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(component.categoryId).toBe(1);
    expect(categoryService.getCategorById).toHaveBeenCalledWith(1);
    expect(component.category).toEqual(mockCategory);
  });

  it('should log "Completed" when category loading completes', () => {
    // Arrange
    const mockResponse: ApiResponse<Category> = { success: true, data: mockCategory, message: '' };
    categoryService.getCategorById.and.returnValue(of(mockResponse));
    spyOn(console, 'log');

    // Act
    fixture.detectChanges();

    // Assert
    expect(console.log).toHaveBeenCalledWith('Completed');
  });
});