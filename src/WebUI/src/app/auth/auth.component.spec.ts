import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthComponent } from './auth.component';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';
import { DummyDataHelper } from 'src/tests/helpers/dummy-data-helper';
import { Subject } from 'rxjs';
import { LoginResult } from '../models/login-result';

describe('AuthComponent', () => {
  let component: AuthComponent;
  let fixture: ComponentFixture<AuthComponent>;
  let authServiceMock: jasmine.SpyObj<AuthService>;
  let routerMock: Partial<Router>;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('authService', ['login']);
    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [ AuthComponent ],
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock}
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form during ngOninit', () => {
    expect(component.form).toBeTruthy();
  });

  it('should call login if form is valid', () => {
    const response = DummyDataHelper.getDummySuccessLoginResult();
    const subject = new Subject<LoginResult>();
    authServiceMock.login.and.returnValue(subject.asObservable());
    subject.next(response);

    component.form.patchValue(DummyDataHelper.getDummyLoginRequest());
    component.onSubmit();
    expect(authServiceMock.login).toHaveBeenCalledTimes(1);
  });

  it('should not call login if form is invalid', () => {
    component.form.patchValue({ nameOrEmail: 'no password'});
    component.onSubmit();
    expect(authServiceMock.login).toHaveBeenCalledTimes(0);
  });

  it('should display backend validation error message', () => {
    const response = DummyDataHelper.getDummyFailLoginResult();
    const subject = new Subject<LoginResult>();
    authServiceMock.login.and.returnValue(subject.asObservable());
    component.form.patchValue(DummyDataHelper.getDummyLoginRequest());

    component.onSubmit();
    subject.next(response);
    expect(authServiceMock.login).toHaveBeenCalledTimes(1);
    expect(component.loginResult?.success).toBeFalse();
    
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    const elem = compiled.querySelector('mat-error') as HTMLElement;
    expect(elem.innerHTML.includes(response.message));
  });
});
