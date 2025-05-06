describe('Register Page', () => {
  it('Đăng ký thành công', () => {
    cy.visit('register');

    const username = 'testuser' + Date.now();
    const email = `${username}@example.com`;
    const password = 'Test1234!';

    cy.get('input[name="username"]').type(username);
    cy.get('input[name="email"]').type(email);
    cy.get('input[name="password"]').type(password);
    cy.get('button[type="submit"]').click();

    // Kiểm tra điều kiện sau khi đăng ký thành công (chuyển hướng / thông báo)
    cy.contains('Đăng ký thành công').should('exist'); // hoặc check redirect
  });
});
