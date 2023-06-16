import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http'
import { AngularMaterialModule } from './angular-material.module';

import { AppRoutingModule } from './app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AuthInterceptor } from './auth/auth.interceptor';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthComponent } from './auth/auth.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { BoardsComponent } from './boards/boards.component';
import { BoardEditComponent } from './boards/board-edit/board-edit.component';
import { BoardCreateComponent } from './boards/board-create/board-create.component';
import { BoardDetailsComponent } from './boards/board-details/board-details.component';
import { StagesComponent } from './stages/stages.component';
import { StageCreateEditComponent } from './stages/stage-create-edit/stage-create-edit.component';
import { AssignmentsComponent } from './assignments/assignments.component';
import { AssignmentViewFullComponent } from './assignments/assignment-view-full/assignment-view-full.component';
import { AssignmentEditComponent } from './assignments/assignment-edit/assignment-edit.component';
import { RegistrationComponent } from './auth/registration/registration.component';
import { EmployeesComponent } from './employees/employees.component';
import { AccountComponent } from './account/account.component';
import { ChangePasswordComponent } from './account/change-password/change-password.component';

@NgModule({
  declarations: [
    AppComponent,
    AuthComponent,
    NavMenuComponent,
    BoardsComponent,
    BoardEditComponent,
    BoardCreateComponent,
    BoardDetailsComponent,
    StagesComponent,
    StageCreateEditComponent,
    AssignmentsComponent,
    AssignmentViewFullComponent,
    AssignmentEditComponent,
    RegistrationComponent,
    EmployeesComponent,
    AccountComponent,
    ChangePasswordComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule, 
    ReactiveFormsModule,
    AngularMaterialModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
