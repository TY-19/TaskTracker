import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { AuthService } from "./auth.service";
import { Router } from "@angular/router";
import { AuthInterceptor } from "./auth.interceptor";
import { TestBed } from "@angular/core/testing";
import { HTTP_INTERCEPTORS, HttpClient } from "@angular/common/http";
import { RouterTestingModule } from "@angular/router/testing";

describe('AuthInterceptor', () => {
    let http: HttpClient;
    let httpMock: HttpTestingController;
    let authService: jasmine.SpyObj<AuthService>;
    let router: Router;

    beforeEach(() => {
      authService = jasmine.createSpyObj('AuthService', ['getToken', 'logout']);

      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule,
          RouterTestingModule
        ],
        providers: [
          AuthInterceptor,
          HttpClient,
          { provide: AuthService, useValue: authService },
          {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
          },
        ],
      });

      http = TestBed.inject(HttpClient);
      httpMock = TestBed.inject(HttpTestingController);
      router = TestBed.inject(Router);
    });

    afterEach(() => {
      httpMock.verify();
    });
  
    it('should add authorization token to the request headers when token is available', () => {
      authService.getToken.and.returnValue('fakeToken');

      http.get('/api/boards').subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne('/api/boards');
      expect(req.request.headers.get('Authorization')).toBe('Bearer fakeToken');
      req.flush({ data: 'fakeData' });
    });
  
    it('should handle HTTP 401 Unauthorized responses by logging out', () => {
      authService.getToken.and.returnValue(null);
      spyOn(router, 'navigate');
      
      http.get('/api/account/profile').subscribe({
        next: () => fail('Expected error, but got success'),
        error: er => expect(er.status).toBe(401)
      });

      const req = httpMock.expectOne('/api/account/profile');
      req.flush(null, { status: 401, statusText: 'Unauthorized' });

      expect(authService.logout).toHaveBeenCalled();
    });
  });