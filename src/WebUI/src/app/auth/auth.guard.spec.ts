import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from "@angular/router";
import { AuthGuard } from "./auth.guard";
import { AuthService } from "./auth.service";
import { TestBed } from "@angular/core/testing";
import { HttpClientTestingModule } from "@angular/common/http/testing";

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let authService: AuthService;
  let routerStub: Partial<Router>;

  beforeEach(() => {
    routerStub = {
      navigate: jasmine.createSpy('navigate')
    };

    TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule ],
      providers: [
        AuthGuard, 
        AuthService,
        { provide: Router, useValue: routerStub }
      ],
    });

    guard = TestBed.inject(AuthGuard);
    authService = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });

  it('should allow access when user is authenticated', () => {
    spyOn(authService, 'isAuthenticated').and.returnValue(true);

    const route: ActivatedRouteSnapshot = {} as ActivatedRouteSnapshot;
    route.data = { roles: [] };
    const state: RouterStateSnapshot = {} as RouterStateSnapshot;

    const result = guard.canActivate(route, state);

    expect(result).toBe(true);
  });

  it('should deny access and redirect to login when user is not authenticated', () => {
    spyOn(authService, 'isAuthenticated').and.returnValue(false);
    

    const route: ActivatedRouteSnapshot = {} as ActivatedRouteSnapshot;
    route.data = { roles: [] };
    const state: RouterStateSnapshot = {} as RouterStateSnapshot;

    const result = guard.canActivate(route, state);

    expect(result).toBe(false);
    expect(routerStub.navigate).toHaveBeenCalled();
  });
});