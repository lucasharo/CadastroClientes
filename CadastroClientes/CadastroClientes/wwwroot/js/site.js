$(document).ready(function () {
    $('.cep').mask('00000-000');
    $('.telefone').mask('(00) 0000-00009');
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.uf').mask('AA');
});
