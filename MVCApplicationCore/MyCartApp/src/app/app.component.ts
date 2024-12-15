import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { LocalstorageService } from './services/helpers/localstorage.service';
import { LocalStorageKeys } from './services/helpers/localstoragekeys';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  // title = 'MyCartApp';
  isAuthenticated: boolean = false;
  username: string | null | undefined;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef, private router: Router) { }
  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges() // Manually trigget change detection.
    });

    this.authService.getUsername().subscribe((username: string | null | undefined) => {
      this.username = username;
      this.cdr.detectChanges() // Manually trigget change detection.
    });
  }
  // signOut() {
  //   this.authService.signOut();
  //   this.router.navigate(['/home']);
  // }
}