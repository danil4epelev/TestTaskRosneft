PGDMP         6                {            TestTask    15.3    15.2 !    !           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            "           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            #           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            $           1262    16398    TestTask    DATABASE     ~   CREATE DATABASE "TestTask" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE "TestTask";
                postgres    false            �            1259    16406    design_objects    TABLE     �   CREATE TABLE public.design_objects (
    id integer NOT NULL,
    name character varying NOT NULL,
    code character varying NOT NULL,
    project_id integer NOT NULL,
    parent_object_id integer
);
 "   DROP TABLE public.design_objects;
       public         heap    postgres    false            �            1259    16423    design_object_id_seq    SEQUENCE     �   ALTER TABLE public.design_objects ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.design_object_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    215            �            1259    16416    directory_brands    TABLE     �   CREATE TABLE public.directory_brands (
    id integer NOT NULL,
    code character varying NOT NULL,
    name character varying NOT NULL
);
 $   DROP TABLE public.directory_brands;
       public         heap    postgres    false            �            1259    16421    directory_brands_id_seq    SEQUENCE     �   ALTER TABLE public.directory_brands ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.directory_brands_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    216            �            1259    16424    package_documentations    TABLE     �   CREATE TABLE public.package_documentations (
    id integer NOT NULL,
    design_object integer NOT NULL,
    brand integer NOT NULL
);
 *   DROP TABLE public.package_documentations;
       public         heap    postgres    false            �            1259    16441    package_documentation_id_seq    SEQUENCE     �   ALTER TABLE public.package_documentations ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.package_documentation_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    220            �            1259    16399    projects    TABLE     �   CREATE TABLE public.projects (
    id integer NOT NULL,
    name character varying NOT NULL,
    code character varying NOT NULL
);
    DROP TABLE public.projects;
       public         heap    postgres    false            �            1259    16422    project_id_seq    SEQUENCE     �   ALTER TABLE public.projects ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.project_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    214            �            1259    16471    users    TABLE     �   CREATE TABLE public.users (
    id integer NOT NULL,
    login character varying NOT NULL,
    hashpassword character varying NOT NULL
);
    DROP TABLE public.users;
       public         heap    postgres    false            �            1259    16470    users_id_seq    SEQUENCE     �   ALTER TABLE public.users ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    223                      0    16406    design_objects 
   TABLE DATA           V   COPY public.design_objects (id, name, code, project_id, parent_object_id) FROM stdin;
    public          postgres    false    215   �%                 0    16416    directory_brands 
   TABLE DATA           :   COPY public.directory_brands (id, code, name) FROM stdin;
    public          postgres    false    216   %&                 0    16424    package_documentations 
   TABLE DATA           J   COPY public.package_documentations (id, design_object, brand) FROM stdin;
    public          postgres    false    220   �&                 0    16399    projects 
   TABLE DATA           2   COPY public.projects (id, name, code) FROM stdin;
    public          postgres    false    214   �&                 0    16471    users 
   TABLE DATA           8   COPY public.users (id, login, hashpassword) FROM stdin;
    public          postgres    false    223   ''       %           0    0    design_object_id_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.design_object_id_seq', 3, true);
          public          postgres    false    219            &           0    0    directory_brands_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.directory_brands_id_seq', 3, true);
          public          postgres    false    217            '           0    0    package_documentation_id_seq    SEQUENCE SET     K   SELECT pg_catalog.setval('public.package_documentation_id_seq', 1, false);
          public          postgres    false    221            (           0    0    project_id_seq    SEQUENCE SET     <   SELECT pg_catalog.setval('public.project_id_seq', 4, true);
          public          postgres    false    218            )           0    0    users_id_seq    SEQUENCE SET     :   SELECT pg_catalog.setval('public.users_id_seq', 1, true);
          public          postgres    false    222            |           2606    16410 !   design_objects Design_object_pkey 
   CONSTRAINT     a   ALTER TABLE ONLY public.design_objects
    ADD CONSTRAINT "Design_object_pkey" PRIMARY KEY (id);
 M   ALTER TABLE ONLY public.design_objects DROP CONSTRAINT "Design_object_pkey";
       public            postgres    false    215            z           2606    16403    projects Project_pkey 
   CONSTRAINT     U   ALTER TABLE ONLY public.projects
    ADD CONSTRAINT "Project_pkey" PRIMARY KEY (id);
 A   ALTER TABLE ONLY public.projects DROP CONSTRAINT "Project_pkey";
       public            postgres    false    214            ~           2606    16420 &   directory_brands directory_brands_pkey 
   CONSTRAINT     d   ALTER TABLE ONLY public.directory_brands
    ADD CONSTRAINT directory_brands_pkey PRIMARY KEY (id);
 P   ALTER TABLE ONLY public.directory_brands DROP CONSTRAINT directory_brands_pkey;
       public            postgres    false    216            �           2606    16446 1   package_documentations package_documentation_pkey 
   CONSTRAINT     o   ALTER TABLE ONLY public.package_documentations
    ADD CONSTRAINT package_documentation_pkey PRIMARY KEY (id);
 [   ALTER TABLE ONLY public.package_documentations DROP CONSTRAINT package_documentation_pkey;
       public            postgres    false    220            �           2606    16475    users users_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    223            �           2606    16452    package_documentations brand    FK CONSTRAINT     �   ALTER TABLE ONLY public.package_documentations
    ADD CONSTRAINT brand FOREIGN KEY (brand) REFERENCES public.directory_brands(id) NOT VALID;
 F   ALTER TABLE ONLY public.package_documentations DROP CONSTRAINT brand;
       public          postgres    false    3198    216    220            �           2606    16447 $   package_documentations design_object    FK CONSTRAINT     �   ALTER TABLE ONLY public.package_documentations
    ADD CONSTRAINT design_object FOREIGN KEY (design_object) REFERENCES public.design_objects(id) NOT VALID;
 N   ALTER TABLE ONLY public.package_documentations DROP CONSTRAINT design_object;
       public          postgres    false    3196    215    220            �           2606    16465 "   design_objects fk_parent_object_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.design_objects
    ADD CONSTRAINT fk_parent_object_id FOREIGN KEY (parent_object_id) REFERENCES public.design_objects(id) NOT VALID;
 L   ALTER TABLE ONLY public.design_objects DROP CONSTRAINT fk_parent_object_id;
       public          postgres    false    215    3196    215            �           2606    16411    design_objects fk_project_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.design_objects
    ADD CONSTRAINT fk_project_id FOREIGN KEY (project_id) REFERENCES public.projects(id) NOT VALID;
 F   ALTER TABLE ONLY public.design_objects DROP CONSTRAINT fk_project_id;
       public          postgres    false    215    3194    214               2   x�3�I-.�O����1~\F�iii�������%� 7����� T9         �   x�%���0D��)� H�:SR�*��S$�+hD�X�n#������.^�pwIܣ��I�D�����w�z������V(�&��l��7���j�	�G�J0�*~�6�.o�(���Aɑ�C��a�o�f���s�            x������ � �         ;   x�3�I-.1�	6�2�-HI,I�R��)�\&�%�� T�ZZT	DE%��\1z\\\ F�H         .   x�3�tL�����O1�
�I��
	�+w1	K�1

������� �
�     