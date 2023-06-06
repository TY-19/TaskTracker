import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthComponent } from './auth/auth.component';
import { AuthGuard } from './auth/auth.guard';
import { AppComponent } from './app.component';
import { BoardsComponent } from './boards/boards.component';

const routes: Routes = [
  // { path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthGuard] },
  { path: 'login', component: AuthComponent },
  { path: 'boards', component: BoardsComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
