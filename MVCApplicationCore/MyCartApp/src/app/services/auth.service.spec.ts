import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { ApiResponse } from '../models/ApiResponse{T}';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LocalstorageService } from './helpers/localstorage.service';
import { LocalStorageKeys } from './helpers/localstoragekeys';
import { User } from '../models/user.model';
import { tap, zip } from 'rxjs';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let localStorageHelper: jasmine.SpyObj<LocalstorageService>;

  const username = 'testuser';
  const password = 'password123';
  const mockUser: User = {
    userId: 0,
    loginId: 'testuser',
    password: 'password123',
    confirmPassword: 'password123',
    email: 'testuser@example.com',
    firstName: 'test',
    lastName: 'user',
    contactNumber: '1234567890',
  };

  beforeEach(() => {
    const localStorageHelperSpy = jasmine.createSpyObj('LocalstorageService', ['removeItem', 'hasItem', 'getItem', 'setItem']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: LocalstorageService, useValue: localStorageHelperSpy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorageHelper = TestBed.inject(LocalstorageService) as jasmine.SpyObj<LocalstorageService>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('signIn', () => {
    it('should sign in successfully', (done: DoneFn) => {
      // Arrange
      const mockSuccessResponse: ApiResponse<string> = {
        success: true,
        data: 'mock-token',
        message: ''
      };
      localStorageHelper.hasItem.and.returnValue(true);
      localStorageHelper.getItem.and.returnValue(username);

      // Act
      service.signIn(username, password).subscribe(response => {
        // Assert
        expect(response).toEqual(mockSuccessResponse);
        expect(localStorageHelper.setItem).toHaveBeenCalledWith(LocalStorageKeys.TokenName, 'mock-token');
        expect(localStorageHelper.setItem).toHaveBeenCalledWith(LocalStorageKeys.UserId, username);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5144/api/Auth/Login');
      expect(req.request.method).toBe('POST');
      req.flush(mockSuccessResponse);
    });

    it('should handle failed sign in', (done: DoneFn) => {
      // Arrange
      const mockErrorResponse: ApiResponse<string> = {
        success: false,
        data: '',
        message: 'Invalid credentials'
      };
      localStorageHelper.hasItem.and.returnValue(false);

      // Act
      service.signIn(username, password).subscribe(response => {
        // Assert
        expect(response).toEqual(mockErrorResponse);
        expect(localStorageHelper.setItem).not.toHaveBeenCalled();
        done();
      });

      const req = httpMock.expectOne('http://localhost:5144/api/Auth/Login');
      expect(req.request.method).toBe('POST');
      req.flush(mockErrorResponse);
    });

    it('should handle HTTP error', (done: DoneFn) => {
      // Arrange
      const mockHttpError = {
        status: 500,
        statusText: 'Internal Server Error'
      };
      localStorageHelper.hasItem.and.returnValue(false);

      // Act
      service.signIn(username, password).subscribe({
        next: () => fail('should have failed with the 500 error'),
        error: (error) => {
          // Assert
          expect(error.status).toBe(500);
          expect(error.statusText).toBe('Internal Server Error');
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5144/api/Auth/Login');
      expect(req.request.method).toBe('POST');
      req.flush({}, mockHttpError);
    });
  });

  describe('signUp', () => {
    it('should register a user successfully', (done: DoneFn) => {
      // Arrange
      const mockSuccessResponse: ApiResponse<string> = {
        success: true,
        data: 'User registered successfully',
        message: ''
      };

      // Act
      service.signUp(mockUser).subscribe(response => {
        // Assert
        expect(response).toEqual(mockSuccessResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5144/api/Auth/Register');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockUser);
      req.flush(mockSuccessResponse);
    });

    it('should handle failed registration', (done: DoneFn) => {
      // Arrange
      const mockErrorResponse: ApiResponse<string> = {
        success: false,
        data: '',
        message: 'Registration failed'
      };

      // Act
      service.signUp(mockUser).subscribe(response => {
        // Assert
        expect(response).toEqual(mockErrorResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5144/api/Auth/Register');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockUser);
      req.flush(mockErrorResponse);
    });

    it('should handle HTTP error during registration', (done: DoneFn) => {
      // Arrange
      const mockHttpError = {
        status: 500,
        statusText: 'Internal Server Error'
      };

      // Act
      service.signUp(mockUser).subscribe({
        next: () => fail('should have failed with the 500 error'),
        error: (error) => {
          // Assert
          expect(error.status).toBe(500);
          expect(error.statusText).toBe('Internal Server Error');
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5144/api/Auth/Register');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockUser);
      req.flush({}, mockHttpError);
    });
  });

  describe('signOut', () => {
    it('should sign out the user and update authState and usernameSubject', (done: DoneFn) => {
      // Arrange
      localStorageHelper.hasItem.and.returnValue(false);
      let isAuthenticatedResult: boolean;
      let getUsernameSubjectResult: string | null | undefined;

      // Act
      service.signOut();

      // Assert
      const authState$ = service.isAuthenticated().pipe(
        tap(authState => isAuthenticatedResult = authState)
      );

      const usernameSubject$ = service.getUsernameSubject().pipe(
        tap(username => getUsernameSubjectResult = username)
      );

      zip(authState$, usernameSubject$).subscribe(() => {
        // Assert after both observables have emitted a value
        expect(localStorageHelper.removeItem).toHaveBeenCalledWith(LocalStorageKeys.TokenName);
        expect(localStorageHelper.removeItem).toHaveBeenCalledWith(LocalStorageKeys.UserId);
        expect(localStorageHelper.hasItem).toHaveBeenCalledWith(LocalStorageKeys.TokenName);
        expect(isAuthenticatedResult).toBe(false);
        expect(getUsernameSubjectResult).toBe(null);
        done();
      });
    });

    describe('isAuthenticated', () => {
      it('should return false when authState is false', (done: DoneFn) => {
        // Arrange
        localStorageHelper.hasItem.and.returnValue(false);
        service.signOut();

        // Act
        service.isAuthenticated().subscribe(authState => {
          // Assert
          expect(authState).toBe(false);
          done();
        });
      });

      it('should return true when authState is true', (done: DoneFn) => {
        // Arrange
        localStorageHelper.hasItem.and.returnValue(true);
        service['authState'].next(true); // Directly set authState for testing

        // Act
        service.isAuthenticated().subscribe(authState => {
          // Assert
          expect(authState).toBe(true);
          done();
        });
      });

      it('should reflect changes in authState', (done: DoneFn) => {
        // Arrange
        localStorageHelper.hasItem.and.returnValue(true);
        service['authState'].next(true); // Set initial state to true

        // Act
        const authStateValues: boolean[] = [];
        service.isAuthenticated().subscribe(authState => {
          authStateValues.push(authState);
          if (authStateValues.length === 1) {
            // Change state to false after first emission
            service['authState'].next(false);
          } else {
            // Assert
            expect(authStateValues).toEqual([true, false]);
            done();
          }
        });
      });
    });

    describe('getUsername', () => {
      it('should return the initial username value from localStorageHelper', (done: DoneFn) => {
        // Arrange
        const mockUsername = 'testUser';
        localStorageHelper.getItem.and.returnValue(mockUsername);
        service = new AuthService(localStorageHelper, {} as any); // Reinitialize service to apply the mock

        // Act
        service.getUsername().subscribe(username => {
          // Assert
          expect(username).toBe(mockUsername);
          done();
        });
      });

      it('should return null if no username is set in localStorageHelper', (done: DoneFn) => {
        // Arrange
        localStorageHelper.getItem.and.returnValue(null);
        service = new AuthService(localStorageHelper, {} as any); // Reinitialize service to apply the mock

        // Act
        service.getUsername().subscribe(username => {
          // Assert
          expect(username).toBeNull();
          done();
        });
      });

      it('should return undefined if usernameSubject is initially undefined', (done: DoneFn) => {
        // Arrange
        localStorageHelper.getItem.and.returnValue(undefined);
        service['usernameSubject'].next(undefined);

        // Act
        service.getUsername().subscribe(username => {
          // Assert
          expect(username).toBeUndefined();
          done();
        });
      });

      it('should reflect changes in usernameSubject', (done: DoneFn) => {
        // Arrange
        const initialUsername = 'initialUser';
        const updatedUsername = 'updatedUser';
        localStorageHelper.getItem.and.returnValue(initialUsername);
        service['usernameSubject'].next(initialUsername); // Set initial value

        // Act
        const usernameValues: (string | null | undefined)[] = [];
        service.getUsername().subscribe(username => {
          usernameValues.push(username);
          if (usernameValues.length === 1) {
            // Change username after first emission
            service['usernameSubject'].next(updatedUsername);
          } else {
            // Assert
            expect(usernameValues).toEqual([initialUsername, updatedUsername]);
            done();
          }
        });
      });
    });
  });
});