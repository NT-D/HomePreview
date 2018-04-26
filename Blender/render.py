import bpy

def delete_all():
    for item in bpy.context.scene.objects:
        bpy.context.scene.objects.unlink(item)

    for item in bpy.data.objects:
        bpy.data.objects.remove(item)

    for item in bpy.data.meshes:
        bpy.data.meshes.remove(item)

    for item in bpy.data.materials:
        bpy.data.materials.remove(item)

delete_all()

bpy.ops.mesh.primitive_cube_add(view_align=False, enter_editmode=False, location=(0, 0, 0))
bpy.ops.transform.resize(value=(1.93489, 1.93489, 1.93489), constraint_axis=(True, False, False), constraint_orientation='GLOBAL', mirror=False, proportional='DISABLED', proportional_edit_falloff='SMOOTH', proportional_size=1, release_confirm=True, use_accurate=False)
bpy.ops.transform.resize(value=(3.64592, 3.64592, 3.64592), constraint_axis=(False, True, False), constraint_orientation='GLOBAL', mirror=False, proportional='DISABLED', proportional_edit_falloff='SMOOTH', proportional_size=1, release_confirm=True, use_accurate=False)

# Get material
mat = bpy.data.materials.get("Material")
if mat is None:
    mat = bpy.data.materials.new(name="Material")
    mat.diffuse_color = (0.617472, 0.8, 0.669385)

#Assign it to active object
ob = bpy.context.active_object
if ob.data.materials:
    ob.data.materials[0] = mat
else:
    ob.data.materials.append(mat)

bpy.ops.object.lamp_add(type='POINT', view_align=False, location=(0.808976, -1.74825, -0.0252551), layers=(True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False))
bpy.ops.object.camera_add(view_align=True, enter_editmode=False, location=(-1.86062, 3.64564, 0.103658), rotation=(1.52174, 0.0126654, -2.60585), layers=(True, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False))
bpy.ops.transform.rotate(value=-5.99116, axis=(0, 0, 1), constraint_axis=(False, False, True), constraint_orientation='GLOBAL', mirror=False, proportional='DISABLED', proportional_edit_falloff='SMOOTH', proportional_size=1, release_confirm=True, use_accurate=False)
bpy.context.object.data.sensor_width = 100

bpy.context.scene.camera = bpy.data.objects['Camera']
bpy.ops.render.render()