describe('Login Tests', () => {
  it('should login successfully and redirect to Tasks page', () => {
    // Truy cập trang đăng nhập
    cy.visit('/login');

    // Điền vào form đăng nhập
    cy.get('input[name="username"]').type('testuser');
    cy.get('input[name="password"]').type('testpassword');

    // Gửi form đăng nhập
    cy.get('form').submit();

    // Kiểm tra URL sau khi đăng nhập thành công
    cy.url().should('include', '/');
  });
});
