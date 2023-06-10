import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthComponent } from './auth/auth.component';
import { AuthGuard } from './auth/auth.guard';
import { BoardsComponent } from './boards/boards.component';
import { BoardEditComponent } from './boards/board-edit/board-edit.component';
import { DefaultRolesNames } from './config/default-roles-names';
import { BoardCreateComponent } from './boards/board-create/board-create.component';
import { BoardDetailsComponent } from './boards/board-details/board-details.component';

const routes: Routes = [
  { path: '', component: BoardsComponent, pathMatch: 'full' },
  { path: 'login', component: AuthComponent },
  { path: 'boards', component: BoardsComponent, canActivate: [AuthGuard] },
  { path: 'boards/:id', component: BoardDetailsComponent, canActivate: [AuthGuard] },
  { path: 'boards/create', component: BoardCreateComponent, canActivate: [AuthGuard],
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
  { path: 'boards/:id/edit', component: BoardEditComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
