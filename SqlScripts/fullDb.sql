--
-- PostgreSQL database dump
--

-- Dumped from database version 13.1
-- Dumped by pg_dump version 13.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: fn_document_delete(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_document_delete(id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		DELETE FROM documents WHERE id = id;
	END;$$;


ALTER FUNCTION public.fn_document_delete(id integer) OWNER TO postgres;

--
-- Name: fn_document_get_by_id(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_document_get_by_id(doc_id integer) RETURNS TABLE(id integer, name character varying, description character varying, insert_user_id integer, insert_date_time time without time zone, modify_user_id integer, modify_date_time time without time zone)
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		RETURN QUERY 
		SELECT d.id, d.name, d.description, d.insert_user_id, d.insert_date_time, d.modify_user_id, d.modify_date_time
		FROM documents d
		WHERE d.id = doc_id;
	END;$$;


ALTER FUNCTION public.fn_document_get_by_id(doc_id integer) OWNER TO postgres;

--
-- Name: fn_document_insert(character varying, text, character varying[], integer, time without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_document_insert(name character varying, description text, categories character varying[], user_id integer, date_time time without time zone) RETURNS integer
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		INSERT INTO documents (id, name, description, categories, insert_user_id, insert_date_time) 
		VALUES (default, name, description, categories, user_id, date_time)
		RETURNING id;
	END;$$;


ALTER FUNCTION public.fn_document_insert(name character varying, description text, categories character varying[], user_id integer, date_time time without time zone) OWNER TO postgres;

--
-- Name: fn_document_update(character varying, text, character varying[], integer, time without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_document_update(name character varying, description text, categories character varying[], modify_user_id integer, modify_date_time time without time zone) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN 
	UPDATE documents 
	SET name = name, description = description, categories = categories, modify_user_id = modify_user_id, modify_date_time = modify_date_time
	WHERE id = id;
END;
$$;


ALTER FUNCTION public.fn_document_update(name character varying, description text, categories character varying[], modify_user_id integer, modify_date_time time without time zone) OWNER TO postgres;

--
-- Name: fn_group_user_delete(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_group_user_delete(group_id integer, user_id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		DELETE FROM group_users WHERE group_id = group_id AND user_id = user_id;
	END;$$;


ALTER FUNCTION public.fn_group_user_delete(group_id integer, user_id integer) OWNER TO postgres;

--
-- Name: fn_group_user_insert(integer, integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_group_user_insert(group_id integer, user_id integer) RETURNS integer
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		INSERT INTO group_users (group_id, user_id) 
		VALUES (group_id, user_id)
		RETURNING id;
	END;$$;


ALTER FUNCTION public.fn_group_user_insert(group_id integer, user_id integer) OWNER TO postgres;

--
-- Name: fn_user_delete(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_delete(id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		DELETE FROM users WHERE id = id;
	END;$$;


ALTER FUNCTION public.fn_user_delete(id integer) OWNER TO postgres;

--
-- Name: fn_user_get_by_id(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_get_by_id(user_id integer) RETURNS TABLE(id integer, email_address character varying, password character varying, active boolean)
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		RETURN QUERY 
		SELECT u.id, u.email_address, u.password, u.active 
		FROM users u
		WHERE u.id = user_id;
	END;$$;


ALTER FUNCTION public.fn_user_get_by_id(user_id integer) OWNER TO postgres;

--
-- Name: fn_user_get_by_name(character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_get_by_name(user_name character varying) RETURNS TABLE(id integer, email_address character varying, password character varying, active boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN 
		RETURN QUERY 
		SELECT u.id, u.email_address, u.password, u.active 
		FROM users u
		WHERE u.email_address = user_name;
	END;
$$;


ALTER FUNCTION public.fn_user_get_by_name(user_name character varying) OWNER TO postgres;

--
-- Name: fn_user_insert(character varying, character varying, boolean, character varying[]); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_insert(email_address character varying, password character varying, active boolean, roles character varying[]) RETURNS integer
    LANGUAGE plpgsql
    AS $$
DECLARE new_id integer;
BEGIN 
	INSERT INTO users (email_address, password, active) 
	VALUES (email_address, password, active)
	RETURNING id INTO new_id;

	INSERT INTO user_roles (user_id, role)
	SELECT new_id, r
	FROM unnest(roles) r;
	
	return new_id;
END;
$$;


ALTER FUNCTION public.fn_user_insert(email_address character varying, password character varying, active boolean, roles character varying[]) OWNER TO postgres;

--
-- Name: fn_user_role_delete(integer, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_role_delete(user_id integer, role character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$ 
BEGIN 
	DELETE FROM user_roles
	WHERE user_id = user_id
		AND role = role;
END;$$;


ALTER FUNCTION public.fn_user_role_delete(user_id integer, role character varying) OWNER TO postgres;

--
-- Name: fn_user_role_insert(integer, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_role_insert(user_id integer, role character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		INSERT INTO user_roles (id, user_id, role) 
		VALUES (default, user_id, role)
		ON CONFLICT (user_id) WHERE role = role DO NOTHING
		RETURNING id;
	END;$$;


ALTER FUNCTION public.fn_user_role_insert(user_id integer, role character varying) OWNER TO postgres;

--
-- Name: fn_user_roles_get(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_roles_get(u_id integer) RETURNS TABLE(role character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN 
		RETURN QUERY 
		SELECT r.role
		FROM user_roles r
		WHERE r.user_id = u_id;
	END;
$$;


ALTER FUNCTION public.fn_user_roles_get(u_id integer) OWNER TO postgres;

--
-- Name: fn_user_update(integer, character varying, boolean, character varying); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_update(id_param integer, email_address_param character varying, active_param boolean, password_param character varying DEFAULT NULL::character varying) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN 
	UPDATE users
	SET email_address = email_address_param,
	active = active_param
	WHERE id = id_param;
	
	IF password_param IS NOT NULL THEN
		UPDATE users 
		SET password = password_param
		WHERE id = id_param;
	END IF;
END;
$$;


ALTER FUNCTION public.fn_user_update(id_param integer, email_address_param character varying, active_param boolean, password_param character varying) OWNER TO postgres;

--
-- Name: fn_user_user_groups_get(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_user_user_groups_get(u_id integer) RETURNS TABLE(id integer, name character varying, active boolean)
    LANGUAGE plpgsql
    AS $$
BEGIN 
	RETURN QUERY 
	SELECT g.id, g.name, g.active
	FROM user_groups g
	INNER JOIN group_users u on g.id = u.group_id
	WHERE u.user_id = u_id;
END;
$$;


ALTER FUNCTION public.fn_user_user_groups_get(u_id integer) OWNER TO postgres;

--
-- Name: fn_usergroup_delete(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_usergroup_delete(id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$ 
BEGIN 
	DELETE FROM group_users WHERE group_id = id;
	DELETE FROM user_groups WHERE id = id;
END;$$;


ALTER FUNCTION public.fn_usergroup_delete(id integer) OWNER TO postgres;

--
-- Name: fn_usergroup_get(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_usergroup_get(group_id integer) RETURNS TABLE(id integer, name character varying, active boolean)
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		RETURN QUERY 
		SELECT g.id, g.name, g.active 
		FROM user_groups g
		WHERE g.id = group_id;
	END;$$;


ALTER FUNCTION public.fn_usergroup_get(group_id integer) OWNER TO postgres;

--
-- Name: fn_usergroup_insert(character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_usergroup_insert(name character varying, active boolean) RETURNS integer
    LANGUAGE plpgsql
    AS $$ 
	BEGIN 
		INSERT INTO user_groups (name, active) 
		VALUES (name, active)
		RETURNING id;
	END;$$;


ALTER FUNCTION public.fn_usergroup_insert(name character varying, active boolean) OWNER TO postgres;

--
-- Name: fn_usergroup_update(integer, character varying, boolean); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.fn_usergroup_update(id integer, name character varying, active boolean) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN 
	UPDATE user_groups 
	SET name = name,
	active = active
	WHERE id = id;
END;
$$;


ALTER FUNCTION public.fn_usergroup_update(id integer, name character varying, active boolean) OWNER TO postgres;

--
-- Name: sp_document_delete(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_document_delete(id integer)
    LANGUAGE sql
    AS $$
DELETE FROM documents WHERE id = id
$$;


ALTER PROCEDURE public.sp_document_delete(id integer) OWNER TO postgres;

--
-- Name: sp_document_get(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_document_get(id integer)
    LANGUAGE sql
    AS $$
SELECT id, name, description, categories, insert_user_id, insert_date_time, modify_user_id, modify_date_time
FROM documents
WHERE id = id
$$;


ALTER PROCEDURE public.sp_document_get(id integer) OWNER TO postgres;

--
-- Name: sp_document_insert(character varying, text, character varying[], integer, time without time zone); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_document_insert(name character varying, description text, categories character varying[], user_id integer, date_time time without time zone)
    LANGUAGE sql
    AS $$
INSERT INTO documents (id, name, description, categories, insert_user_id, insert_date_time) 
VALUES (default, name, description, categories, user_id, date_time)
RETURNING id;
$$;


ALTER PROCEDURE public.sp_document_insert(name character varying, description text, categories character varying[], user_id integer, date_time time without time zone) OWNER TO postgres;

--
-- Name: sp_document_update(character varying, text, character varying[], integer, time without time zone); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_document_update(name character varying, description text, categories character varying[], modify_user_id integer, modify_date_time time without time zone)
    LANGUAGE sql
    AS $$
UPDATE documents 
SET name = name, description = description, categories = categories, modify_user_id = modify_user_id, modify_date_time = modify_date_time
WHERE id = id
$$;


ALTER PROCEDURE public.sp_document_update(name character varying, description text, categories character varying[], modify_user_id integer, modify_date_time time without time zone) OWNER TO postgres;

--
-- Name: sp_group_user_delete(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_group_user_delete(group_id integer, user_id integer)
    LANGUAGE sql
    AS $$
DELETE FROM group_users WHERE group_id = group_id AND user_id = user_id;
$$;


ALTER PROCEDURE public.sp_group_user_delete(group_id integer, user_id integer) OWNER TO postgres;

--
-- Name: sp_group_user_insert(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_group_user_insert(group_id integer, user_id integer)
    LANGUAGE sql
    AS $$
INSERT INTO group_users (group_id, user_id) 
VALUES (group_id, user_id)
$$;


ALTER PROCEDURE public.sp_group_user_insert(group_id integer, user_id integer) OWNER TO postgres;

--
-- Name: sp_group_users_insert(integer, integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_group_users_insert(group_id integer, user_id integer)
    LANGUAGE sql
    AS $$
INSERT INTO group_users (group_id, user_id) 
VALUES (group_id, user_id)
$$;


ALTER PROCEDURE public.sp_group_users_insert(group_id integer, user_id integer) OWNER TO postgres;

--
-- Name: sp_user_delete(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_delete(id integer)
    LANGUAGE sql
    AS $$
DELETE FROM users WHERE id = id
$$;


ALTER PROCEDURE public.sp_user_delete(id integer) OWNER TO postgres;

--
-- Name: sp_user_get_by_id(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_get_by_id(id integer)
    LANGUAGE sql
    AS $$
SELECT id, email_address, password, active 
FROM users
WHERE id = id
$$;


ALTER PROCEDURE public.sp_user_get_by_id(id integer) OWNER TO postgres;

--
-- Name: sp_user_get_by_name(character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_get_by_name(username character varying)
    LANGUAGE sql
    AS $$
SELECT id, email_address, password, active 
FROM users
WHERE email_address = username
$$;


ALTER PROCEDURE public.sp_user_get_by_name(username character varying) OWNER TO postgres;

--
-- Name: sp_user_insert(character varying, character varying, boolean, character varying[]); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_insert(email_address character varying, password character varying, active boolean, roles character varying[])
    LANGUAGE sql
    AS $$
INSERT INTO users (email_address, password, active, roles) 
VALUES (email_address, password, active, roles)
RETURNING id;
$$;


ALTER PROCEDURE public.sp_user_insert(email_address character varying, password character varying, active boolean, roles character varying[]) OWNER TO postgres;

--
-- Name: sp_user_insert(character varying, character varying, boolean, character varying[], integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_insert(email_address character varying, password character varying, active boolean, roles character varying[], INOUT id integer)
    LANGUAGE sql
    AS $$
INSERT INTO users (email_address, password, active, roles) 
VALUES (email_address, password, active, roles)
RETURNING id;
$$;


ALTER PROCEDURE public.sp_user_insert(email_address character varying, password character varying, active boolean, roles character varying[], INOUT id integer) OWNER TO postgres;

--
-- Name: sp_user_role_delete(integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_role_delete(user_id integer, role character varying)
    LANGUAGE sql
    AS $$
DELETE FROM user_roles
WHERE user_id = user_id
	AND role = role
$$;


ALTER PROCEDURE public.sp_user_role_delete(user_id integer, role character varying) OWNER TO postgres;

--
-- Name: sp_user_role_insert(integer, character varying[]); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_role_insert(user_id integer, roles character varying[])
    LANGUAGE sql
    AS $$
INSERT INTO user_roles (user_id, role) 
SELECT user_id, unnest(roles)
ON CONFLICT (user_id) WHERE role = role DO NOTHING
$$;


ALTER PROCEDURE public.sp_user_role_insert(user_id integer, roles character varying[]) OWNER TO postgres;

--
-- Name: sp_user_role_insert(integer, character varying); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_role_insert(user_id integer, role character varying)
    LANGUAGE sql
    AS $$
INSERT INTO user_roles (id, user_id, role) 
VALUES (default, user_id, role)
ON CONFLICT (user_id) WHERE role = role DO NOTHING
$$;


ALTER PROCEDURE public.sp_user_role_insert(user_id integer, role character varying) OWNER TO postgres;

--
-- Name: sp_user_roles_get(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_roles_get(user_id integer)
    LANGUAGE sql
    AS $$
SELECT role FROM user_roles WHERE user_id = user_id
$$;


ALTER PROCEDURE public.sp_user_roles_get(user_id integer) OWNER TO postgres;

--
-- Name: sp_user_update(integer, character varying, character varying, boolean); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_user_update(id integer, email_address character varying, password character varying, active boolean)
    LANGUAGE sql
    AS $$
UPDATE users 
SET email_address = email_address, 
	password = password, 
	active = active
WHERE id = id
$$;


ALTER PROCEDURE public.sp_user_update(id integer, email_address character varying, password character varying, active boolean) OWNER TO postgres;

--
-- Name: sp_usergroup_delete(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_usergroup_delete(id integer)
    LANGUAGE sql
    AS $$
BEGIN TRANSACTION;
	DELETE FROM group_users WHERE group_id = id;
	DELETE FROM user_groups WHERE id = id;
COMMIT;
$$;


ALTER PROCEDURE public.sp_usergroup_delete(id integer) OWNER TO postgres;

--
-- Name: sp_usergroup_get(integer); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_usergroup_get(id integer)
    LANGUAGE sql
    AS $$
SELECT id, name, active 
FROM user_groups
WHERE id = id
$$;


ALTER PROCEDURE public.sp_usergroup_get(id integer) OWNER TO postgres;

--
-- Name: sp_usergroup_insert(character varying, boolean); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_usergroup_insert(name character varying, active boolean)
    LANGUAGE sql
    AS $$
INSERT INTO user_groups (name, active) 
VALUES (name, active)
RETURNING id;
$$;


ALTER PROCEDURE public.sp_usergroup_insert(name character varying, active boolean) OWNER TO postgres;

--
-- Name: sp_usergroup_update(integer, character varying, boolean); Type: PROCEDURE; Schema: public; Owner: postgres
--

CREATE PROCEDURE public.sp_usergroup_update(id integer, name character varying, active boolean)
    LANGUAGE sql
    AS $$
UPDATE user_groups 
SET name = name,
	active = active
WHERE id = id
$$;


ALTER PROCEDURE public.sp_usergroup_update(id integer, name character varying, active boolean) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: documents; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.documents (
    id integer NOT NULL,
    name character varying(500) NOT NULL,
    description text,
    insert_user_id integer NOT NULL,
    insert_date_time time without time zone NOT NULL,
    modify_user_id integer,
    modify_date_time time without time zone,
    categories character varying[]
);


ALTER TABLE public.documents OWNER TO postgres;

--
-- Name: Documents_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.documents ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Documents_Id_seq"
    START WITH 10000
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: document_access_entities; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.document_access_entities (
    id integer NOT NULL,
    document_id integer NOT NULL,
    entity_id integer NOT NULL,
    insert_user_id integer NOT NULL,
    insert_date_time time without time zone NOT NULL,
    entity_type_id integer NOT NULL
);


ALTER TABLE public.document_access_entities OWNER TO postgres;

--
-- Name: document_access_entity_types; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.document_access_entity_types (
    id integer NOT NULL,
    name character varying(150) NOT NULL
);


ALTER TABLE public.document_access_entity_types OWNER TO postgres;

--
-- Name: document_categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.document_categories (
    name character varying(150) NOT NULL,
    id integer NOT NULL,
    acitve boolean NOT NULL,
    description character varying(1000)
);


ALTER TABLE public.document_categories OWNER TO postgres;

--
-- Name: group_users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.group_users (
    group_id integer NOT NULL,
    user_id integer NOT NULL
);


ALTER TABLE public.group_users OWNER TO postgres;

--
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    id integer NOT NULL,
    name character varying(150) NOT NULL,
    description character varying(500)
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- Name: roles_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.roles ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.roles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: user_groups; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_groups (
    id integer NOT NULL,
    name character varying(150) NOT NULL,
    active boolean NOT NULL
);


ALTER TABLE public.user_groups OWNER TO postgres;

--
-- Name: user_groups_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.user_groups ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.user_groups_id_seq
    START WITH 100
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: user_roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_roles (
    id integer NOT NULL,
    user_id integer NOT NULL,
    role character varying NOT NULL
);


ALTER TABLE public.user_roles OWNER TO postgres;

--
-- Name: user_roles_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.user_roles ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.user_roles_id_seq
    START WITH 200
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    id integer NOT NULL,
    email_address character varying(150) NOT NULL,
    password character varying(500) NOT NULL,
    active boolean NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.users ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.users_id_seq
    START WITH 186
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: document_access_entities; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.document_access_entities (id, document_id, entity_id, insert_user_id, insert_date_time, entity_type_id) FROM stdin;
\.


--
-- Data for Name: document_access_entity_types; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.document_access_entity_types (id, name) FROM stdin;
\.


--
-- Data for Name: document_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.document_categories (name, id, acitve, description) FROM stdin;
\.


--
-- Data for Name: documents; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.documents (id, name, description, insert_user_id, insert_date_time, modify_user_id, modify_date_time, categories) FROM stdin;
\.


--
-- Data for Name: group_users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.group_users (group_id, user_id) FROM stdin;
\.


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roles (id, name, description) FROM stdin;
1	administrator	full CRUD permissions
2	manager	document upload and download permissions
\.


--
-- Data for Name: user_groups; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_groups (id, name, active) FROM stdin;
\.


--
-- Data for Name: user_roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_roles (id, user_id, role) FROM stdin;
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (id, email_address, password, active) FROM stdin;
\.


--
-- Name: Documents_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Documents_Id_seq"', 10000, false);


--
-- Name: roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_id_seq', 2, true);


--
-- Name: user_groups_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_groups_id_seq', 101, true);


--
-- Name: user_roles_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_roles_id_seq', 200, true);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_id_seq', 197, true);


--
-- Name: document_access_entities DocumentAccessEntities_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.document_access_entities
    ADD CONSTRAINT "DocumentAccessEntities_pkey" PRIMARY KEY (id);


--
-- Name: document_access_entity_types DocumentAccessEntityTypes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.document_access_entity_types
    ADD CONSTRAINT "DocumentAccessEntityTypes_pkey" PRIMARY KEY (id);


--
-- Name: document_categories DocumentCategories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.document_categories
    ADD CONSTRAINT "DocumentCategories_pkey" PRIMARY KEY (id);


--
-- Name: documents Documents_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT "Documents_pkey" PRIMARY KEY (id);


--
-- Name: user_groups Groups_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_groups
    ADD CONSTRAINT "Groups_pkey" PRIMARY KEY (id);


--
-- Name: roles Roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT "Roles_pkey" PRIMARY KEY (id);


--
-- Name: users Users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT "Users_pkey" PRIMARY KEY (id);


--
-- Name: group_users group_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.group_users
    ADD CONSTRAINT group_users_pkey PRIMARY KEY (group_id, user_id);


--
-- Name: user_roles user_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_roles
    ADD CONSTRAINT user_roles_pkey PRIMARY KEY (id);


--
-- Name: fki_FK_InsertUserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "fki_FK_InsertUserId" ON public.documents USING btree (insert_user_id);


--
-- Name: fki_FK_ModifiyUserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "fki_FK_ModifiyUserId" ON public.documents USING btree (modify_user_id);


--
-- Name: ix_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_user_id ON public.user_roles USING btree (user_id);


--
-- Name: document_access_entities FK_DocumentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.document_access_entities
    ADD CONSTRAINT "FK_DocumentId" FOREIGN KEY (document_id) REFERENCES public.documents(id) NOT VALID;


--
-- Name: documents FK_InsertUserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT "FK_InsertUserId" FOREIGN KEY (insert_user_id) REFERENCES public.users(id) NOT VALID;


--
-- Name: documents FK_ModifiyUserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.documents
    ADD CONSTRAINT "FK_ModifiyUserId" FOREIGN KEY (modify_user_id) REFERENCES public.users(id) NOT VALID;


--
-- Name: user_roles fk_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_roles
    ADD CONSTRAINT fk_user_id FOREIGN KEY (user_id) REFERENCES public.users(id) NOT VALID;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

GRANT ALL ON SCHEMA public TO "DemoApi";


--
-- Name: TABLE documents; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.documents TO "DemoApi";


--
-- Name: TABLE document_access_entities; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.document_access_entities TO "DemoApi";


--
-- Name: TABLE document_access_entity_types; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.document_access_entity_types TO "DemoApi";


--
-- Name: TABLE document_categories; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.document_categories TO "DemoApi";


--
-- Name: TABLE group_users; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.group_users TO "DemoApi";


--
-- Name: TABLE roles; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.roles TO "DemoApi";


--
-- Name: TABLE user_groups; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.user_groups TO "DemoApi";


--
-- Name: TABLE user_roles; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.user_roles TO "DemoApi";


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE public.users TO "DemoApi";


--
-- PostgreSQL database dump complete
--

