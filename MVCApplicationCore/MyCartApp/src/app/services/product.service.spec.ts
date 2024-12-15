import { TestBed } from '@angular/core/testing';
import { ProductService } from './product.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing'
import { ApiResponse } from '../models/ApiResponse{T}';
import { Product } from '../models/product.model';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock: HttpTestingController;
  const mockApiResponse: ApiResponse<Product[]> = {
    success: true,
    message: '',
    data: [
      {
        productId: 1,
        productName: "Product 1",
        productDescription: "Product Description 1",
        categoryId: 1,
        productPrice: 552.56,
        category: {
          categoryId: 1,
          description: "Description 1",
          name: "Category 1",
          fileName: "filename.jpg"
        },
        inStock: true,
        isActive: true
      },
      {
        productId: 2,
        productName: "Product 2",
        productDescription: "Product Description 2",
        categoryId: 2,
        productPrice: 552.56,
        category: {
          categoryId: 2,
          description: "Description 2",
          name: "Category 2",
          fileName: "filename1.jpg"
        },
        inStock: true,
        isActive: true
      }]
  };


  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService]
    });
    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all products successfully', () => {
    // Arrange
    const apiUrl = 'http://localhost:5144/api/Product/'
    // Act
    service.getAllProducts().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl + "GetAllProducts");
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });

  it('should handle an empty product list', () => {
    // Arrange
    const apiUrl = 'http://localhost:5144/api/Product/';
    const emptyResponse: ApiResponse<Product[]> = {
      success: true,
      data: [],
      message: ''
    };

    //  Act
    service.getAllProducts().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });

    const req = httpMock.expectOne(apiUrl + "GetAllProducts");
    expect(req.request.method).toBe("GET");
    req.flush(emptyResponse);
  });

  it('should handle HTTP error gracefully', () => {
    // Arrange
    const apiUrl = 'http://localhost:5144/api/Product/';
    const errorMessage = "Failed to load products";

    // Act
    service.getAllProducts().subscribe(
      () => fail('expected an error, not products'),
      (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );
    const req = httpMock.expectOne(apiUrl + "GetAllProducts");
    expect(req.request.method).toBe("GET");
    req.flush(errorMessage, { status: 500, statusText: 'Internal Server Error' });
  });
});