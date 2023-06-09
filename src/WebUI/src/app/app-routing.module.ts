import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthComponent } from './auth/auth.component';
import { AuthGuard } from './auth/auth.guard';
import { BoardsComponent } from './boards/boards.component';
import { BoardEditComponent } from './boards/board-edit/board-edit.component';
import { DefaultRolesNames } from './config/default-roles-names';

const routes: Routes = [
  { path: '', component: BoardsComponent, pathMatch: 'full' },
  { path: 'login', component: AuthComponent },
  { path: 'boards', component: BoardsComponent, canActivate: [AuthGuard] },
  { path: 'boards/:id/edit', component: BoardEditComponent, canActivate: [AuthGuard], 
    data: { roles: [DefaultRolesNames.DEFAULT_ADMIN_ROLE, DefaultRolesNames.DEFAULT_MANAGER_ROLE] } },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
