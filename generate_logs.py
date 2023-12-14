tables = ['dish', 'ingredient', 'menu', 
						'order', 'payment', 'reservation', 
                        'role', 'table', 'user', 
                        'ingredient_dish','dish_menu', 'table_reservation',
                        'order_waiter', 'dish_order']
CRUD = ['create', 'update', 'delete']
ins = 'insert'
cre = 'create'
dele = 'delete'
old = 'OLD'
new = 'NEW'
def add_s(string: str) -> str:
    if '_' in string:
        pos = string.index('_')
        return string[:pos] + select_s(string[:pos]) + string[string.index('_'):] + select_s(string[pos + 1:])
    return string + select_s(string)

def select_s(string: str) -> str:
    return 's' if string != 'dish' else 'es'


file = open('loggs.sql','w')
for crud in CRUD:
    for type in tables:
        file.write(f'DROP TRIGGER IF EXISTS {crud}_{type}_log;\nDELIMITER //\nCREATE TRIGGER {crud}_{type}_log\n')
        file.write(f'\tBEFORE {str.upper(ins) if crud == cre else str.upper(crud)} ON restaurant.{add_s(type)}\n\tFOR EACH ROW\n\tBEGIN\n')
        file.write(f'\t\tCALL logger(@CUR_ID, \'{crud}\', \'{type}\', {old if crud == dele else new}.{type}_id);\n')
        file.write(f'\tEND //\nDELIMITER ;\n\n')
file.close()


file = open('select.sql', 'w')
for creature in tables:
    file.write(f'DROP PROCEDURE IF EXISTS get_{add_s(creature)};\nDELIMITER //\n')
    file.write(f'CREATE PROCEDURE get_{add_s(creature)}()\nBEGIN\n')
    file.write(f'\t SELECT * FROM restaurant.{add_s(creature)};\nEND //\nDELIMITER ;\n\n')
file.close()


file = open('select_id.sql', 'w')
for creature in tables:
    file.write(f'DROP PROCEDURE IF EXISTS get_{creature}_by_id;\nDELIMITER //\n')
    file.write(f'CREATE PROCEDURE get_{creature}_by_id(id_{creature} INT UNSIGNED)\nBEGIN\n')
    file.write(f'\tSELECT * FROM restaurant.{add_s(creature)}\n\tWHERE {creature}_id = id_{creature};\nEND //\nDELIMITER ;\n\n')
file.close()

file = open('delete_id.sql', 'w')
for creature in tables:
    file.write(f'DROP PROCEDURE IF EXISTS delete_{creature};\nDELIMITER //\n')
    file.write(f'CREATE PROCEDURE delete_{creature}(id_{creature} INT UNSIGNED)\nBEGIN\n')
    file.write(f'\DELETE FROM restaurant.{add_s(creature)}\n\tWHERE {creature}_id = id_{creature};\nEND //\nDELIMITER ;\n\n')
file.close()