import { TestBed } from '@angular/core/testing';

import { CategoryService } from './category.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Category } from '../models/category.model.';
import { AddCategory } from '../models/add-category.model';

describe('CategoryService', () => {
  let service: CategoryService;
  let httpMock: HttpTestingController;
  const mockApiResponse: ApiResponse<Category[]> = {
    success: true,
    data: [
      {
        categoryId: 1,
        categoryName: 'Category 1',
        categoryDescription: 'Description 1'
      },
      {
        categoryId: 2,
        categoryName: 'Category 2',
        categoryDescription: 'Description 2'
      }
    ],
    message: ''
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CategoryService]
    });

    service = TestBed.inject(CategoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all categories successfully', () => {
    // Arrange
    const apiUrl = 'http://localhost:5144/api/Category/GetAllCategories';

    // Act
    service.getAllCategories().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });

  it('should handle an empty categories list', () => {
    // Arrange
    const apiUrl = 'http://localhost:5144/api/Category/GetAllCategories';

    const emptyResponse: ApiResponse<Category[]> = {
      success: true,
      data: [],
      message: ''
    }

    // Act
    service.getAllCategories().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle HTTP error gracefully', () => {
    // Arrange
    const apiUrl = 'http://localhost:5144/api/Category/GetAllCategories';
    const errorMessage = 'Failed to load categories';

    // Act
    service.getAllCategories().subscribe(
      () => fail('expected an error, not categories'),
      (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');

    // Respond with error
    req.flush(errorMessage, { status: 500, statusText: 'Internal Server Error' });
  });

  it('should add a category successfully', () => {
    // Arrange
    const addCategory: AddCategory = {
      categoryName: 'new category',
      categoryDescription: 'Description of the new category'
    }

    const mockSuccessResponse: ApiResponse<string> = {
      success: true,
      message: "Category saved successfully.",
      data: ""
    };

    // Act
    service.AddCategory(addCategory).subscribe(response => {
      // Assert
      expect(response).toBe(mockSuccessResponse);

    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/Create');
    expect(req.request.method).toBe('POST');
    req.flush(mockSuccessResponse);

  });

  it('should handle failed category addition', () => {
    // Arrange
    const addCategory: AddCategory = {
      categoryName: 'new category',
      categoryDescription: 'Description of the new category'
    }

    const mockErrorResponse: ApiResponse<string> = {
      success: false,
      message: "Category already exists.",
      data: ""
    };

    // Act
    service.AddCategory(addCategory).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);

    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/Create');
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse);

  });
  it('should handle http error', () => {
    // Arrange
    const addCategory: AddCategory = {
      categoryName: 'new category',
      categoryDescription: 'Description of the new category'
    };

    const mockHttpError = {
      status: 500,
      statusText: "Internal Server Error",
    };

    // Act
    service.AddCategory(addCategory).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error) => {
        // Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/Create');
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });

  it('should update a category successfully', () => {
    // Arrange
    const updatedCategory: Category = {
      categoryId: 1,
      categoryName: 'Updated category',
      categoryDescription: "updated description"
    };

    const mockSuccessResponse: ApiResponse<string> = {
      success: true,
      data: '',
      message: 'Category updated successfully.'
    };

    // Act
    service.updateCategory(updatedCategory).subscribe(
      response => {
        // Assert
        expect(response).toEqual(mockSuccessResponse);
      });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/ModifyCategory');
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });
  it('should handle failed category update', () => {
    // Arrange
    const updatedCategory: Category = {
      categoryId: 1,
      categoryName: 'Updated Category',
      categoryDescription: 'Updated description',
    };

    const mockErrorResponse: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Failed to update category'
    };

    // Act
    service.updateCategory(updatedCategory).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/ModifyCategory');
    expect(req.request.method).toBe('PUT');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error', () => {
    // Arrange
    const updatedCategory: Category = {
      categoryId: 1,
      categoryName: 'Updated Category',
      categoryDescription: 'Updated description',
    };

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act
    service.updateCategory(updatedCategory).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/ModifyCategory');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });

  it('should fetch a category by id successfully', () => {
    // Arrange
    const categoryId = 1;
    const mockSuccessResponse: ApiResponse<Category> = {
      success: true,
      data: {
        categoryId: 1,
        categoryName: 'Category 1',
        categoryDescription: 'Description of category 1',
      },
      message: ''
    };

    // Act
    service.getCategorById(categoryId).subscribe(response => {
      // Assert
      expect(response.success).toBeTrue();
      expect(response.message).toBe('');
      expect(response.data).toEqual(mockSuccessResponse.data);
      expect(response.data.categoryId).toEqual(categoryId);
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/GetCategoryById/' + categoryId);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  });

  it('should handle failed category retrival', () => {
    // Arrange
    const categoryId = 1;
    const mockErrorResponse: ApiResponse<Category> = {
      success: false,
      data: {} as Category,
      message: "No record found!"
    };

    // Act
    service.getCategorById(categoryId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.message).toEqual("No record found!");
      expect(response.success).toBeFalse();
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/GetCategoryById/' + categoryId);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);

  });

  it('should handle HTTP errors', () => {
    // Arrange
    const categoryId = 1;
    const mockHttpError = {
      status: 500,
      statusText: "Internal Server Error"
    };

    // Act
    service.getCategorById(categoryId).subscribe({
      next: () => fail('should have failed with 500 error'),
      error: (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/GetCategoryById/' + categoryId);
    expect(req.request.method).toBe('GET');
    req.flush({}, mockHttpError);
  });

  it('should delete a category by id successfully', () => {
    // Arrange
    const categoryId = 1;
    const mockSuccessResponse: ApiResponse<string> = {
      success: true,
      data: '',
      message: 'Category deleted successfully.'
    };

    // Act
    service.deleteCategorById(categoryId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockSuccessResponse);

    });
    const req = httpMock.expectOne('http://localhost:5144/api/Category/Remove/' + categoryId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockSuccessResponse);
  });

  it('should handle failed category deletion', () => {
    // Arrange
    const categoryId = 1;
    const mockErrorResponse: ApiResponse<string> = {
      success: false,
      data: '',
      message: 'Failed to delete category'
    };

    // Act
    service.deleteCategorById(categoryId).subscribe(response => {
      // Assert
      expect(response).toEqual(mockErrorResponse);
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/Remove/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error', () => {
    // Arrange
    const categoryId = 1;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    // Act
    service.deleteCategorById(categoryId).subscribe({
      next: () => fail('should have failed with the 500 error'),
      error: (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5144/api/Category/Remove/1');
    expect(req.request.method).toBe('DELETE');
    req.flush({}, mockHttpError);
  });
});
