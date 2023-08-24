import { Component } from '@angular/core';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'tt-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor(private authService: AuthService) {

  }

  ngOnInit(): void {
    this.authService.init();
  }
}
