import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FooterMenuComponent } from './footer-menu.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('FooterMenuComponent', () => {
  let component: FooterMenuComponent;
  let fixture: ComponentFixture<FooterMenuComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[RouterTestingModule],
      declarations: [FooterMenuComponent]
    });
    fixture = TestBed.createComponent(FooterMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
